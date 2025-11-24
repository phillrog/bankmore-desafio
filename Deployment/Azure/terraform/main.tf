# ====================================================================
# TERRAFORM CONFIGURATION: AKS and SQL Deployment
# ====================================================================

# ----------------------------------------------------
# 1. CONFIGURAÇÃO DE PROVEDORES
# ----------------------------------------------------

terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 3.0"
    }
    null = {
      source  = "hashicorp/null"
      version = "~> 3.0"
    }
  }
}

provider "azurerm" {
  features {}
}

# ----------------------------------------------------
# 2. GERAÇÃO DE RECURSOS BÁSICOS E LOCALS 
# ----------------------------------------------------

resource "azurerm_resource_group" "rg" {
  name     = "rg-${var.prefix}-aks-sql"
  location = var.location
}

locals {
  # Base da Connection String SQL, usada para o script de criação de DBs e output.
  sql_conn_base = "Server=tcp:${azurerm_mssql_server.sql_server.fully_qualified_domain_name},1433;Persist Security Info=False;User ID=${var.sql_connection_details.user};Password=${var.sql_connection_details.password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

  # Mapeamento para criação de DBs
  app_db_connection_map = {
    "Api.Identidaide.DefaultConnection"             = "BankMoreIdentidadeDB"
    "Api.ContasCorrentes.DefaultConnection"         = "BankMoreContaCorrenteDB"
    "Api.Transferencia.DefaultConnection"           = "BankMoreTransferenciaDB"
    "Api.Transferencia.BankMoreContaCorrenteDBConnection" = "BankMoreContaCorrenteDB"
  }

  # 🎯 Connection Strings completas para injeção no K8s
  db_identidade_conn    = "${local.sql_conn_base}Database=${local.app_db_connection_map["Api.Identidaide.DefaultConnection"]}"
  db_contas_conn        = "${local.sql_conn_base}Database=${local.app_db_connection_map["Api.ContasCorrentes.DefaultConnection"]}"
  db_transferencia_conn = "${local.sql_conn_base}Database=${local.app_db_connection_map["Api.Transferencia.DefaultConnection"]}"

  kafka_endpoint        = "broker:29092"
  aggregate_sql_connections = "${local.db_identidade_conn}|${local.db_contas_conn}|${local.db_transferencia_conn}"
}

# ----------------------------------------------------
# 3. AZURE SQL SERVER E EXECUÇÃO DO SCRIPT DE BANCOS
# ----------------------------------------------------

resource "azurerm_mssql_server" "sql_server" {
  name                          = var.sql_connection_details.fqdn
  resource_group_name           = azurerm_resource_group.rg.name
  location                      = azurerm_resource_group.rg.location
  version                       = "12.0" 
  administrator_login           = var.sql_connection_details.user
  administrator_login_password  = var.sql_connection_details.password
  minimum_tls_version           = "1.2"
  public_network_access_enabled = true 

  tags = {
    environment = var.prefix
  }
}

resource "azurerm_mssql_firewall_rule" "firewall_rule" {
  name                = "AllowAzureAndLocal"
  server_id           = azurerm_mssql_server.sql_server.id
  start_ip_address    = "0.0.0.0" 
  end_ip_address      = "0.0.0.0"
}

# ----------------------------------------------------
# GARANTINDO PERMISSÕES DOS SCRIPTS (CHMOD)
# ----------------------------------------------------
resource "null_resource" "setup_file_permissions" {
  # Adiciona um 'trigger' para garantir que o comando seja executado.
  triggers = {
    always_run = timestamp()
  }

  provisioner "local-exec" {
    command = <<EOT
      echo "Definindo permissões de execução para scripts shell..."
      # Permissão de execução para todos os scripts .sh no diretório 'terraform'
      sed -i 's/\\r$//' *.sh && chmod +x *.sh
    EOT
    interpreter = ["/bin/bash", "-c"]
  }
}

# Cria os bancos de dados após o servidor estar provisionado.
resource "null_resource" "create_sql_databases" {
  depends_on = [
    azurerm_mssql_server.sql_server, 
    azurerm_mssql_firewall_rule.firewall_rule,
    null_resource.setup_file_permissions # ⬅️ DEPENDÊNCIA CORRIGIDA (com vírgula)
  ]

  provisioner "local-exec" {
    # Adiciona 'sleep 30' e usa o caminho correto para o script shell
    command = "sleep 30 && ./create_db.sh ${azurerm_mssql_server.sql_server.fully_qualified_domain_name} ${var.sql_connection_details.user} ${var.sql_connection_details.password}"
    interpreter = ["/bin/bash", "-c"]
  }
}


# ----------------------------------------------------
# 4. AZURE KUBERNETES SERVICE (AKS)
# ----------------------------------------------------

resource "azurerm_kubernetes_cluster" "aks" {
  name                = "aks-${var.prefix}-cluster"
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
  
  tags = {
    environment = var.prefix
  }
}

# ----------------------------------------------------
# 5. CREDENCIAL WAITER (BAIXA O KUBECONFIG)
# ----------------------------------------------------

resource "null_resource" "aks_credentials_waiter" {
  triggers = {
    always_run = timestamp()
  }

  depends_on = [azurerm_kubernetes_cluster.aks]

  provisioner "local-exec" {
    # Comando para baixar as credenciais do AKS
    command       = "az aks get-credentials --resource-group ${azurerm_resource_group.rg.name} --name ${azurerm_kubernetes_cluster.aks.name} --overwrite-existing --file ~/.kube/config"
    interpreter   = ["/bin/bash", "-c"]
  }
}

# ----------------------------------------------------
# 6. DEPLOYMENT DO KAFKA E APIS VIA KUBECTL
# ----------------------------------------------------

resource "null_resource" "deploy_kafka_to_aks" {
  triggers = {
    aks_cluster_name = azurerm_kubernetes_cluster.aks.name
  }

  depends_on = [
    azurerm_kubernetes_cluster.aks,
    null_resource.aks_credentials_waiter,
    null_resource.setup_file_permissions # Garante que o script tenha permissão de execução
  ]

  provisioner "local-exec" {
    # Comando passa as três Connection Strings resolvidas para o script shell
    command = "echo 'Aguardando 20 segundos para a estabilidade do AKS API Server...' && sleep 20 && ./terraform/deploy_kafka.sh \"${local.db_identidade_conn}\" \"${local.db_contas_conn}\" \"${local.db_transferencia_conn}\""
    interpreter = ["/bin/bash", "-c"]
  }
}