#!/bin/bash
#
# Script para criar bancos de dados no Azure SQL Server usando sqlcmd.
#

# Parâmetros de entrada
SQL_SERVER_FQDN=$1
SQL_USER=$2
SQL_PASSWORD=$3

# Nomes dos bancos de dados a serem criados
DB_NAMES=("BankMoreIdentidadeDB" "BankMoreContaCorrenteDB" "BankMoreTransferenciaDB")

# Lista dos arquivos .sql
SQL_FILES=(
    "identidadedb.sql"
    "contacorrentedb.sql"
    "transferenciadb.sql"
)

echo "--- Iniciando criação de Bancos de Dados no Servidor: $SQL_SERVER_FQDN ---"
echo "--- Esperando 10 segundos para garantir que o Firewall e o Servidor SQL estejam ativos... ---"
sleep 10

# Loop pelos nomes dos bancos e seus respectivos arquivos
for i in "${!DB_NAMES[@]}"; do
    DB_NAME=${DB_NAMES[i]}
    SQL_FILE=../dbs${SQL_FILES[i]}

    # 1. Criação do Banco de Dados
    echo "Criando o banco de dados: $DB_NAME ..."

    echo "Banco de Dados $DB_NAME criado com sucesso."

    # 2. Execução do Script SQL (se o arquivo existir)
    if [ -f "$SQL_FILE" ]; then
        echo "Executando o script $SQL_FILE no banco $DB_NAME..."
        # Comando exatamente como solicitado:
        sqlcmd -S "$SQL_SERVER_FQDN" -d "$DB_NAME" -U "$SQL_USER" -P "$SQL_PASSWORD" -i "$SQL_FILE"

        if [ $? -eq 0 ]; then
            echo "Script $SQL_FILE executado com sucesso no $DB_NAME."
        else
            echo "ERRO ao executar o script $SQL_FILE no $DB_NAME."
        fi
    else
        echo "Atenção: Arquivo de script SQL $SQL_FILE não encontrado. Pulando execução de schema/dados."
    fi

done

echo "--- Processo de criação de Bancos de Dados Concluído. ---"