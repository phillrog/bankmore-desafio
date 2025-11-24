# ----------------------------------------------------
# VARIÁVEIS DE RECURSO AZURE
# ----------------------------------------------------

variable "prefix" {
  description = "Prefixo usado para nomear recursos (ex: 'bankmore')."
  type        = string
}

variable "location" {
  description = "Região do Azure onde os recursos serão implantados (ex: 'centralus')."
  type        = string
  default     = "centralus"
}

# ----------------------------------------------------
# VARIÁVEIS DO SQL SERVER (Credenciais e FQDN)
# ----------------------------------------------------

variable "sql_connection_details" {
  description = "Detalhes de conexão, FQDN e credenciais para o SQL Server."
  type = object({
    fqdn     = string
    user     = string
    password = string
  })
  sensitive = true
}

# ----------------------------------------------------
# VARIÁVEIS DO KUBERNETES 
# ----------------------------------------------------

variable "aks_node_count" {
  description = "Número de nós padrão no cluster AKS."
  type        = number
  default     = 2
}

# ----------------------------------------------------
# VARIÁVEL DE MAPEAMENTO DE SERVIÇOS (Mantida para o Output do FQDN do DB)
# ----------------------------------------------------

variable "api_services_map" {
  description = "Mapeamento das APIs para criar as Connection Strings SQL no Secret."
  type = map(object({
    port = number
  }))
  default = {
    "identidade" = {
      port = 5000
    },
    "contascorrentes" = {
      port = 5001
    },
    "transferencias" = {
      port = 5002
    }
  }
}