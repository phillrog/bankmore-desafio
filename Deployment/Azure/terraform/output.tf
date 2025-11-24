output "sql_connection_strings_all" {
  description = "Todas as Connection Strings do SQL, separadas por um pipe (|)"
  value       = local.aggregate_sql_connections
  sensitive   = true 
}

output "kafka_bootstrap_server" {
  description = "Endpoint do Kafka (interno ao AKS)"
  value       = local.kafka_endpoint
}

output "aks_cluster_name" {
  description = "Nome do Cluster AKS"
  value       = azurerm_kubernetes_cluster.aks.name
}