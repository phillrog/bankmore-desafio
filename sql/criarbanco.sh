# Variáveis de Conexão
SA_PASSWORD='yourStrong(!)Password'
SERVER='mssql'
TIMEOUT=90 # Aumentando o tempo limite para 90 segundos
DB_READY_SIGNAL="/tmp/db_ready"

# 1. Loop de Espera Ativo (Aumentando o timeout de ping para 5s)
echo "Aguardando o SQL Server ficar online e pronto para aceitar comandos..."
for i in $(seq 1 $TIMEOUT); do
    # Tenta executar um comando SQL simples
    /opt/mssql-tools/bin/sqlcmd \
        -S "$SERVER" \
        -U sa \
        -P "$SA_PASSWORD" \
        -Q "SELECT 1" \
        -l 90 \
        &> /dev/null
    
    if [ $? -eq 0 ]; then
        echo "SQL Server está pronto após $i segundos."
        touch $DB_READY_SIGNAL 
        break
    fi
    echo "Aguardando... ($i/$TIMEOUT)"
    sleep 1
done

if [ ! -f $DB_READY_SIGNAL ]; then
    echo "ERRO: O SQL Server não iniciou dentro do tempo limite ($TIMEOUT segundos)."
    exit 1
fi

# 2. Executa todos os arquivos .sql no diretório /scripts
echo "Executando scripts SQL..."

for sql_file in /scripts/*.sql
do 
    echo "  -> Executando arquivo: $sql_file"
    
    # O timeout de execução (-l 30) está OK, pois o servidor já está pingando.
    /opt/mssql-tools/bin/sqlcmd \
        -S "$SERVER" \
        -U sa \
        -P "$SA_PASSWORD" \
        -i "$sql_file" \
        -l 30 \
        -e
    
    if [ $? -ne 0 ]; then
        echo "ERRO: Falha ao executar o script $sql_file. Interrompendo."
        exit 1
    fi
done

echo "Todos os scripts SQL executados com sucesso."


/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'yourStrong(!)Password' -Q 'SELECT name FROM sys.databases;'