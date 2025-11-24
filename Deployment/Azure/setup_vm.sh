#!/bin/bash
# ----------------------------------------------------
# Script: setup_vm.sh
# Descrição: Instala Terraform, Azure CLI, SQLCMD, kubectl e HELM no Ubuntu.
# Uso: chmod +x setup_vm.sh && ./setup_vm.sh
# ----------------------------------------------------

echo "## 🚀 Iniciando o provisionamento de ferramentas na VM Ubuntu..."

# Garante que o PATH inclua o sqlcmd para uso IMEDIATO no script
export PATH="$PATH:/opt/mssql-tools/bin"

# ----------------------------------------------------
# 1. Atualizar o sistema
# ----------------------------------------------------
echo "--- 1. Atualizando pacotes do sistema (apt update) ---"
sudo apt update -y
sudo apt install -y curl unzip wget gnupg software-properties-common apt-transport-https

# ----------------------------------------------------
# 2. Instalar o Terraform
# ----------------------------------------------------
echo "--- 2. Instalando o Terraform ---"
if ! command -v terraform &> /dev/null
then
    echo "Terraform não encontrado. Instalando..."
    # Adicionar a chave GPG da HashiCorp
    curl -fsSL https://apt.releases.hashicorp.com/gpg | sudo gpg --dearmor -o /usr/share/keyrings/hashicorp-archive-keyring.gpg
    
    # Adicionar o repositório oficial da HashiCorp
    echo "deb [signed-by=/usr/share/keyrings/hashicorp-archive-keyring.gpg] https://apt.releases.hashicorp.com $(lsb_release -cs) main" | sudo tee /etc/apt/sources.list.d/hashicorp.list > /dev/null
    
    # Instalar
    sudo apt update
    sudo apt install terraform -y
    
    echo "✅ Terraform instalado com sucesso."
else
    echo "✅ Terraform já está instalado: $(terraform -v | head -n 1)"
fi

# ----------------------------------------------------
# 3. Instalar o Azure CLI
# ----------------------------------------------------
echo "--- 3. Instalando o Azure CLI ---"
if ! command -v az &> /dev/null
then
    echo "Azure CLI não encontrado. Instalando..."
    # Adicionar a chave de assinatura da Microsoft
    curl -sL https://packages.microsoft.com/keys/microsoft.asc | 
        gpg --dearmor | 
        sudo tee /etc/apt/keyrings/microsoft.gpg > /dev/null
    
    # Adicionar o repositório do Azure CLI
    AZ_REPO=$(lsb_release -cs)
    echo "deb [arch=`dpkg --print-architecture` signed-by=/etc/apt/keyrings/microsoft.gpg] https://packages.microsoft.com/repos/azure-cli/ $AZ_REPO main" | 
        sudo tee /etc/apt/sources.list.d/azure-cli.list > /dev/null
    
    # Instalar
    sudo apt update
    sudo apt install azure-cli -y

    echo "✅ Azure CLI instalado com sucesso."
else
    echo "✅ Azure CLI já está instalado: $(az version | grep "azure-cli" | head -n 1)"
fi

# ----------------------------------------------------
# 4. Instalar o SQLCMD (mssql-tools e unixodbc) - CORRIGIDO
# ----------------------------------------------------
echo "--- 4. Instalando o SQLCMD (para criar os bancos) ---"
# Testa a existência física do binário, que é mais confiável que testar o PATH
if [ ! -f /opt/mssql-tools/bin/sqlcmd ]; then
    echo "SQLCMD não encontrado. Instalando..."
    sudo apt install curl
    # Adicionar o repositório do SQL Server (Comando mais conciso e robusto)
    curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
    sudo add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/prod.list)"
    
    # Instalar unixodbc-dev (dependência) e mssql-tools
    sudo apt update
    sudo apt install unixodbc-dev -y
    # A instalação do mssql-tools pode pedir aceitação de licença (ENTER, YES/NO)
    sudo apt install mssql-tools -y 
    
    
    # Adicionar mssql-tools ao PATH (para o usuário atual)
    echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc

    source ~/.bashrc
    
    echo "✅ SQLCMD instalado e adicionado ao PATH. O PATH foi atualizado para este script."
else
    echo "✅ SQLCMD já está instalado."
fi

# ----------------------------------------------------
# 5. Instalar o kubectl (Necessário para a interação com o AKS)
# ----------------------------------------------------
echo "--- 5. Instalando o kubectl (Gerenciamento do Kubernetes) ---"
if ! command -v kubectl &> /dev/null
then
    echo "kubectl não encontrado. Instalando..."
    
    # Baixar a versão mais recente
    curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl"
    
    # Tornar executável e mover para o PATH
    sudo install -o root -g root -m 0755 kubectl /usr/local/bin/kubectl
    rm kubectl # Remover o arquivo temporário
    
    echo "✅ kubectl instalado com sucesso."
else
    echo "✅ kubectl já está instalado: $(kubectl version --client --output=short)"
fi

# ----------------------------------------------------
# 6. Instalar o Helm (Gerenciador de Pacotes do Kubernetes)
# ----------------------------------------------------
echo "--- 6. Instalando o Helm (Gerenciador de Pacotes do Kubernetes) ---"
if ! command -v helm &> /dev/null
then
    echo "Helm não encontrado. Instalando..."
    # Baixa e instala o Helm
    # 1. Baixa o Helm
    curl -fsSL -o get_helm.sh https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3

    # 2. Torna o script de instalação executável
    chmod +x get_helm.sh

    # 3. Executa o script para instalar o Helm (em /usr/local/bin)
    sudo ./get_helm.sh

    # 4. Verifica a instalação
    helm version --short
    
    echo "✅ Helm instalado com sucesso."
else
    echo "✅ Helm já está instalado: $(helm version --short)"
fi


# ----------------------------------------------------
# 7. Verificação Final de Instalação e Versões
# ----------------------------------------------------
echo "--- 7. Verificação Final de Instalação e Versões ---"

# Função para obter a versão ou status de erro
check_version() {
    local command_name="$1"
    local version_command="$2"
    
    # Use o PATH atualizado para este script
    if command -v "$command_name" &> /dev/null; then
        # Executa o comando e pega a primeira linha ou a que interessa.
        echo "🟢 $command_name: $($version_command 2>&1 | head -n 1)"
    else
        echo "🔴 $command_name: FALHOU (Comando não encontrado no PATH)"
    fi
}

check_version "terraform" "terraform --version"
check_version "az" "az --version "
check_version "sqlcmd" "sqlcmd -?" # Comando de versão mais direto
check_version "kubectl" "kubectl version --client --short"
check_version "helm" "helm version --short" # NOVO: Verifica o Helm

# ----------------------------------------------------
# 8. Conclusão e Próximos Passos
# ----------------------------------------------------
echo "--- 8. Conclusão ---"
echo "Todas as ferramentas essenciais foram instaladas e verificadas."
echo " "
echo "## ➡️ PRÓXIMOS PASSOS CRUCIAIS:"
echo "1. **Atualização do PATH:** O PATH do SQLCMD foi atualizado apenas para este script. Para uso no terminal, execute **source ~/.bashrc**."
echo "2. **Autenticar no Azure:** Execute: "
echo "   az login"
echo "3. **Inicializar o Terraform:** Execute: "
echo "   terraform init"
echo "4. **Aplicar a infraestrutura:** Execute: "
echo "   terraform apply"
echo "5. **Instalar o Ingress (DEPOIS do AKS estar de pé):** Execute:"
echo "   helm install nginx-ingress ingress-nginx/ingress-nginx --namespace ingress-nginx --create-namespace"