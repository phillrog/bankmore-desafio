#!/bin/bash
# ----------------------------------------------------
# Script: deploy_kafka.sh
# Descrição: Implanta o ecossistema Kafka/Zookeeper, o NGINX Ingress Controller e as APIs no AKS
# ----------------------------------------------------

echo "## 🚀 Iniciando o deployment do ecossistema Kafka e APIs no AKS..."

K8S_MANIFESTS_DIR="../k8s"

# Verifica se os arquivos YAML existem
if [ ! -f "${K8S_MANIFESTS_DIR}/zookeeper.yaml" ] || \
   [ ! -f "${K8S_MANIFESTS_DIR}/broker.yaml" ] || \
   [ ! -f "${K8S_MANIFESTS_DIR}/kafka-ui.yaml" ] || \
   [ ! -f "${K8S_MANIFESTS_DIR}/init-kafka.yaml" ] || \
   [ ! -f "${K8S_MANIFESTS_DIR}/apis.yaml" ] || \
    [ ! -f "${K8S_MANIFESTS_DIR}/ingress-controller.yaml" ]; then 
    echo "🔴 ERRO: Um ou mais arquivos YAML estão faltando no diretório: ${K8S_MANIFESTS_DIR}"
    exit 1
fi

# 1. Cria o Namespace 'kafka'
echo "--- 1. Criando o Namespace 'kafka' (se não existir)..."
kubectl create namespace kafka --dry-run=client -o yaml | kubectl apply -f -

# NOVO PASSO: 2. Aplica o NGINX Ingress Controller (no namespace ingress-nginx)
echo "--- 2. Aplicando o NGINX Ingress Controller (Cria LoadBalancer e IP Público)..."
# Ele criará o namespace 'ingress-nginx' internamente
kubectl apply -f ${K8S_MANIFESTS_DIR}/ingress-controller.yaml
echo "--- AGUARDANDO o NGINX Ingress Controller obter o IP público (pode levar 1-2 minutos)..."
sleep 10

# 3. Aplica Zookeeper, Broker, e Kafka UI
echo "--- 3. Aplicando Zookeeper, Broker, e Kafka UI no namespace 'kafka'..."
kubectl apply -f ${K8S_MANIFESTS_DIR}/zookeeper.yaml
kubectl apply -f ${K8S_MANIFESTS_DIR}/broker.yaml
kubectl apply -f ${K8S_MANIFESTS_DIR}/kafka-ui.yaml

# 4. Aguarda um tempo para o Broker subir
echo "--- 4. Aguardando 60 segundos para o Kafka Broker inicializar..."
sleep 60

# 5. Executa o Job de Inicialização (criação de tópico)
echo "--- 5. Aplicando Job de Inicialização do Kafka..."
kubectl apply -f ${K8S_MANIFESTS_DIR}/init-kafka.yaml

# 6. Aplica os Deployments e Ingress das APIs
echo "--- 6. Aplicando Deployments e Ingress das BankMore APIs..."
kubectl apply -f ${K8S_MANIFESTS_DIR}/apis.yaml

# 7. Verifica o status final
echo "--- 7. Verificação Final dos recursos nos Namespaces 'kafka' e 'ingress-nginx' ---"
kubectl get all -n kafka
kubectl get svc -n ingress-nginx # Mostra o IP público do LoadBalancer NGINX

echo "## ✅ Deployment do Ingress, Kafka e APIs concluído!"
echo " "
echo "➡️ PRÓXIMO PASSO: Verifique o 'EXTERNAL-IP' do Service 'ingress-nginx-controller' acima."
echo "   Use este IP para testar suas APIs: http://<EXTERNAL-IP>/identidade/swagger"