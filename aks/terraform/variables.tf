# variables.tf

variable "prefix" {
  description = "Prefixo único para nomear recursos, ex: bankmore"
  type        = string
}

variable "location" {
  description = "Região do Azure, ex: East US"
  type        = string
  default     = "East US"
}

variable "sql_admin_password" {
  description = "Senha do administrador do SQL Server"
  type        = string
  sensitive   = true
}

variable "aks_node_count" {
  description = "Número de nós de worker no cluster AKS"
  type        = number
  default     = 2
}

# Mapa de Microsserviços
variable "api_services_map" {
  description = "Mapa de microsserviços e suas portas"
  type = map(object({
    port        = number
    description = string
  }))
  default = {
    "identidade" = {
      port        = 5000
      description = "API de Identidade e Usuários"
    },
    "contascorrentes" = {
      port        = 5001
      description = "API de Contas Correntes"
    },
    "transferencias" = {
      port        = 5002
      description = "API de Transferências"
    }
  }
}