#!/usr/bin/env bash
# Define o shell a ser usado para a execução (neste caso, Bash)

# Configuração de Erros:
# -e: Sai imediatamente se um comando falhar.
# -u: Trata variáveis não definidas como erro e sai.
# -o pipefail: Garante que um pipeline (comandos conectados por |) falhe se qualquer comando na cadeia falhar.
set -euo pipefail

# 1. Preparação do Diretório de Nuget
rm -rf nuget
# Remove o diretório 'nuget' se ele existir (limpeza)
mkdir nuget
# Cria um novo diretório 'nuget' (provavelmente para armazenar pacotes temporários ou cache, embora não seja usado explicitamente abaixo)

cd ./Src
# Muda o diretório atual para a pasta 'Src', onde estão todos os projetos C# (.csproj).
# Todos os comandos subsequentes de 'find', 'pushd' e 'dotnet' serão executados a partir daqui.

# 2. Loop principal de Build
for p in $(find . -name *.csproj);
# 'find . -name *.csproj' procura recursivamente por todos os arquivos .csproj na pasta 'Src' e subpastas.
# O loop 'for' itera sobre o caminho de cada projeto encontrado (ex: ./BankMore.Core/BankMore.Core.csproj).
do
    # 3. Extração e Tratamento de Caminhos
    # Exemplo de entrada para 'p': ./BankMore.Infra.Data.ContasCorrentes/BankMore.Infra.Data.ContasCorrentes.csproj
    
    dir=$(cut -d'/' -f2 <<<"$p")
    # Extrai o nome do diretório principal (ex: BankMore.Infra.Data.ContasCorrentes)
    
    proj=$(cut -d'/' -f3 <<<"$p")
    # Extrai o nome do arquivo .csproj (ex: BankMore.Infra.Data.ContasCorrentes.csproj)
    
    # 4. Tratamento de Estruturas Aninhadas (Ex: Pastas de APIs)
    if [[ $dir == *"APIs"* ]]; then
        # Verifica se o diretório principal contém "APIs" no nome, indicando uma estrutura aninhada (ex: Src/APIs/Identidade/Identidade.csproj)
        
        dir=APIs/$(cut -d'/' -f3 <<<"$p");
        # Reconstrói o caminho do diretório para incluir a pasta 'APIs' (ex: APIs/Identidade)
        
        proj=$(cut -d'/' -f4 <<<"$p")
        # Ajusta a extração para obter o nome do arquivo .csproj correto (quarta parte do caminho)
    fi
    
    # 5. Execução do Build
    pushd ./$dir
    # 'pushd' (Push Directory) salva o diretório atual na pilha e muda para o diretório do projeto (ex: ./Src/BankMore.Infra.Data.ContasCorrentes)
    
    dotnet restore
    # Restaura as dependências (pacotes NuGet) do projeto C# atual.
    
    dotnet build $proj --configuration Release --no-restore;
    # Compila o projeto C# na configuração 'Release'.
    # '--no-restore' é usado pois o 'dotnet restore' já foi executado na linha anterior, economizando tempo.
    
    popd
    # 'popd' (Pop Directory) retorna ao diretório anterior na pilha (volta para ./Src).
done
# Fim do Loop