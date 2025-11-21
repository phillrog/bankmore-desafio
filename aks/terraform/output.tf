output "acr_login_server" {
  description = "Servidor de login do Azure Container Registry para o Docker push/pull"
  value       = azurerm_container_registry.acr.login_server
}

output "load_balancer_ip" {
  description = "IP Público do Azure Load Balancer para acesso HTTP puro (Ex: http://<IP>:5000)"
  value       = kubernetes_service.loadbalancer_service.status[0].load_balancer[0].ingress[0].ip
}

output "sql_server_fqdn" {
  description = "FQDN do Azure SQL Server"
  value       = azurerm_mssql_server.sql_server.fully_qualified_domain_name
}