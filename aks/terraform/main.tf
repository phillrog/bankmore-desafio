# main.tf

# -----------------
# AZURE PROVIDER
# -----------------

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.10"
    }
    random = {
      source  = "hashicorp/random"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# -----------------
# GERAÇÃO DE RECURSOS BÁSICOS E LOCALS (Connection String)
# -----------------

resource "random_string" "suffix" {
  length  = 4
  special = false
  upper   = false
}

resource "azurerm_resource_group" "rg" {
  name     = "rg-${var.prefix}-${random_string.suffix.result}"
  location = var.location
}

# **LOCALS**: Define a string de conexão base para ser usada no ConfigMap e Init Container
locals {
  # String de conexão base que será preenchida com o nome do DB
  sql_conn_base = "Server=tcp:${azurerm_mssql_server.sql_server.fully_qualified_domain_name},1433;Persist Security Info=False;User ID=${azurerm_mssql_server.sql_server.administrator_login};Password=${var.sql_admin_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

  # Mapeamento dos nomes de DB internos (usados pelo Terraform/K8s) para os nomes de Connection String esperados pelo .NET
  db_conn_map = {
    identidade      = "DefaultConnection",
    contascorrentes = "BankMoreContaCorrenteDBConnection",
    transferencias  = "TransferenciaDb"
  }
}

# -----------------
# 1. AZURE CONTAINER REGISTRY (ACR)
# -----------------

resource "azurerm_container_registry" "acr" {
  name                = "acrbankmore${random_string.suffix.result}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Basic"
  admin_enabled       = true
}

# -----------------
# 2. AZURE SQL SERVER E DATABASES
# -----------------

resource "azurerm_mssql_server" "sql_server" {
  name                         = "sql-bankmore-${random_string.suffix.result}"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = azurerm_resource_group.rg.location
  version                      = "12.0"
  administrator_login          = "sqladmin"
  administrator_login_password = var.sql_admin_password
  minimum_tls_version          = "1.2"
}

# 2.1 Firewall Rule: Permite que os serviços Azure (incluindo AKS) se conectem
resource "azurerm_mssql_firewall_rule" "azure_access" {
  name             = "AllowAzureServices"
  server_id        = azurerm_mssql_server.sql_server.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

# 2.2 Criação dos bancos de dados por microsserviço
# Os nomes criados são: db-identidade, db-contascorrentes, db-transferencias
resource "azurerm_mssql_database" "databases" {
  for_each = var.api_services_map
  name     = "db-${each.key}"
  server_id = azurerm_mssql_server.sql_server.id
  collation = "SQL_Latin1_General_CP1_CI_AS"
  sku_name = "Basic"
}


# -----------------
# 3. AZURE KUBERNETES SERVICE (AKS)
# -----------------

resource "azurerm_kubernetes_cluster" "aks" {
  name                = "aks-${var.prefix}-${random_string.suffix.result}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  dns_prefix          = "aks-${var.prefix}"

  default_node_pool {
    name       = "default"
    node_count = var.aks_node_count
    vm_size    = "Standard_B2s"
  }

  identity {
    type = "SystemAssigned"
  }
}

# **Simplificação** da Role Assignment para pull do ACR (uso do SystemAssigned Identity)
resource "azurerm_role_assignment" "acr_pull" {
  scope                = azurerm_container_registry.acr.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_kubernetes_cluster.aks.kubelet_identity[0].object_id
}

# -----------------
# 4. KUBERNETES PROVIDER CONFIGURAÇÃO
# -----------------

data "azurerm_kubernetes_cluster" "aks_data" {
  name                = azurerm_kubernetes_cluster.aks.name
  resource_group_name = azurerm_resource_group.rg.name
}

provider "kubernetes" {
  host                   = data.azurerm_kubernetes_cluster.aks_data.kube_config.0.host
  client_certificate     = base64decode(data.azurerm_kubernetes_cluster.aks_data.kube_config.0.client_certificate)
  client_key             = base64decode(data.azurerm_kubernetes_cluster.aks_data.kube_config.0.client_key)
  cluster_ca_certificate = base64decode(data.azurerm_kubernetes_cluster.aks_data.kube_config.0.cluster_ca_certificate)
}

# -----------------
# 5. KUBERNETES CONFIGMAP (Variáveis de Conexão e Connection Strings)
# -----------------

resource "kubernetes_config_map" "app_config" {
  metadata {
    name = "bankmore-config"
  }

  data = merge(
    {
      ASPNETCORE_ENVIRONMENT = "Production",

      # 1. Variáveis Separadas (Usadas pelo Init Container sqlcmd)
      SQL_SERVER_HOST        = azurerm_mssql_server.sql_server.fully_qualified_domain_name,
      SQL_SERVER_USER        = azurerm_mssql_server.sql_server.administrator_login,
      SQL_SERVER_PASSWORD    = var.sql_admin_password
    },
    # 2. Connection Strings Completas (Usadas pelas APIs .NET)
    {
      for key, conn_name in local.db_conn_map : 
      conn_name => "Initial Catalog=db-${key};${local.sql_conn_base}" 
    }
  )
}

# -----------------
# 6. KUBERNETES IMAGEPULLSECRET (Acesso ao ACR)
# -----------------

resource "kubernetes_secret" "acr_secret" {
  metadata {
    name = "acr-secret"
  }
  type = "kubernetes.io/dockerconfigjson"
  data = {
    ".dockerconfigjson" = jsonencode({
      auths = {
        (azurerm_container_registry.acr.login_server) = {
          username = azurerm_container_registry.acr.admin_username
          password = azurerm_container_registry.acr.admin_password
          auth     = base64encode("${azurerm_container_registry.acr.admin_username}:${azurerm_container_registry.acr.admin_password}")
        }
      }
    })
  }
}

# -----------------
# 7. KUBERNETES SERVICES (Serviços Internos de ClusterIP)
# -----------------

resource "kubernetes_service" "api_services" {
  for_each = var.api_services_map

  metadata {
    name = "svc-${each.key}"
  }

  spec {
    selector = {
      App = "api-${each.key}"
    }

    port {
      port        = each.value.port
      target_port = each.value.port
    }

    type = "ClusterIP"
  }
}

# -----------------
# 8. KUBERNETES DEPLOYMENTS (Com Init Container para DB)
# -----------------

resource "kubernetes_deployment" "api_deployments" {
  for_each = var.api_services_map

  metadata {
    name = "deploy-${each.key}"
    labels = {
      App = "api-${each.key}"
    }
  }

  spec {
    replicas = 1
    selector {
      match_labels = {
        App = "api-${each.key}"
      }
    }

    template {
      metadata {
        labels = {
          App = "api-${each.key}"
        }
      }
      spec {
        
        # --- INIT CONTAINER PARA EXECUÇÃO DO SCRIPT SQL (DB Init) ---
        init_container {
          name  = "db-init-${each.key}"
          
          # Sua imagem deve ser a 'bankmore-db-init:latest' (com o sqlcmd e o script sql.sql)
          image = "${azurerm_container_registry.acr.login_server}/bankmore-db-init:latest" 
          
          command = ["/bin/bash", "-c"]
          args = [
            "export DB_NAME=db-${each.key};" 
            
            "echo 'Aguardando 30s e Inicializando o DB: $DB_NAME'; sleep 30; " 
            
            # Comando sqlcmd: -S (Server), -U (User), -P (Password), -d (Database), -i (Input file)
            "/opt/mssql-tools/bin/sqlcmd "
            "-S $SQL_SERVER_HOST -U $SQL_SERVER_USER -P $SQL_SERVER_PASSWORD "
            "-d $DB_NAME " # Conecta-se ao DB criado pelo Terraform
            "-i /app/sql.sql; " # Executa o script que você forneceu
            
            "if [ $? -eq 0 ]; then echo 'Inicialização do DB $DB_NAME bem-sucedida.'; else echo 'Erro na inicialização do DB $DB_NAME.'; exit 1; fi"
          ]

          # Injeta as variáveis de conexão separadas (HOST, USER, PASS) para o sqlcmd
          env_from {
            config_map_ref {
              name = kubernetes_config_map.app_config.metadata[0].name
            }
          }
        }
        # --- FIM INIT CONTAINER ---
        
        container {
          name  = "api-container"
          # Imagens das suas APIs
          image = "${azurerm_container_registry.acr.login_server}/bankmore-${each.key}:latest" 
          
          port {
            container_port = each.value.port
          }
          
          # Injeta o ConfigMap que contém as Connection Strings COMPLETAS e o ASPNETCORE_ENVIRONMENT
          env_from {
            config_map_ref {
              name = kubernetes_config_map.app_config.metadata[0].name
            }
          }
        }

        image_pull_secrets {
          name = kubernetes_secret.acr_secret.metadata[0].name
        }
      }
    }
  }
}

# -----------------
# 9. KUBERNETES SERVICE LOADBALANCER (Acesso HTTP Simples)
# -----------------

resource "kubernetes_service" "loadbalancer_service" {
  metadata {
    name = "bankmore-http-entrypoint"
    annotations = {
      "service.beta.kubernetes.io/azure-load-balancer-mode" = "managed" 
    }
  }

  spec {
    selector = {
      App = "api-identidade" 
    }
    
    # Expondo as 3 portas separadamente no Load Balancer
    port {
      name        = "http-identidade"
      port        = 5000 
      target_port = 5000
    }
    port {
      name        = "http-contascorrentes"
      port        = 5001 
      target_port = 5001
    }
    port {
      name        = "http-transferencias"
      port        = 5002 
      target_port = 5002
    }
    type = "LoadBalancer"
  }
}