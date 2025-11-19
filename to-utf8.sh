# Define as codificações
FROM_ENCODING="ISO-8859-1"
TO_ENCODING="UTF-8" 

# Caminho dos arquivos a serem processados (ponto atual, busca em subpastas por padrão)
TARGET_DIR="."

# 1. Encontra (recursivamente) e processa todos os arquivos .cs
find "$TARGET_DIR" -type f -name "*.cs" | while read FILE; do
    echo "Convertendo: $FILE"
    
    # 2. Converte para um arquivo temporário
    # Certifique-se de que o 'iconv' está instalado no seu ambiente de execução
    iconv -f "$FROM_ENCODING" -t "$TO_ENCODING" "$FILE" > "$FILE.tmp" 
    
    # 3. Verifica o sucesso e substitui
    if [ $? -eq 0 ]; then
        mv "$FILE.tmp" "$FILE"
        echo "   -> Sucesso."
    else
        # Se houver erro, remove o arquivo temporário
        rm "$FILE.tmp"
        echo "   -> ERRO na conversão de $FILE. Arquivo original mantido."
    fi
done