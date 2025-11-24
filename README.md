# Build

[![Build Clients](https://github.com/phillrog/identityserver4-dot-net-8/actions/workflows/ci-clients.yml/badge.svg)](https://github.com/phillrog/identityserver4-dot-net-8/actions/workflows/ci-clients.yml)

---

# 🏦 Desfio # BankMore: Plataforma de Microsserviços Bancários


Este projeto é a plataforma de serviços bancários digitais, **Deafio - BankMore**, desenvolvida com uma arquitetura de **Microsserviços** desacoplados. Adota fortemente os padrões de **Domain-Driven Design (DDD)** e **CQRS** para gerenciar a complexidade do domínio.

A comunicação e as transações distribuídas são tratadas de forma assíncrona, utilizando **Apache Kafka** e o **Outbox Pattern (Kaflow)**, com o padrão **SAGA** implementado para garantir a consistência das transações de negócio (e.g., Transferências). O projeto garante **Idempotência** e aderência a boas práticas de desenvolvimento.

---

## 🗺️ Overview dos Microsserviços

O sistema é composto por três APIs Web principais e um Worker dedicado:

1.  **BankMore.Services.Api.Identidade:** Responsável por **Cadastro e Autenticação de Usuários** utilizando **Identity Core**, gerando **Tokens JWT** e aplicando **Roles & Policies** para controle de acesso.
2.  **BankMore.Services.Api.ContasCorrentes:** Gerencia contas, movimentações (Depósito/Saque) e consulta de saldo.
3.  **BankMore.Services.Api.Transferencias:** Orquestra a transação de transferência (**SAGA**).
4.  **BankMore.Transferencias.Workers:** Consumidor dedicado de eventos Kafka, crucial para a lógica do SAGA de Transferências.

---

## 🏛️ Architecture & Fluxo de Código (Code Flow)

O projeto segue a arquitetura em camadas do **DDD**, com separação de responsabilidades (Domínio, Aplicação, Infraestrutura). 

### 🔄 Padrão SAGA e Comunicação Assíncrona

A **Transferência** é uma transação distribuída implementada com o padrão **SAGA Orquestrado**.

* O **Outbox Pattern (Kaflow)** é crucial para garantir a atomicidade: o evento é publicado no Kafka somente se a persistência no MSSQL for bem-sucedida.
* O Microsserviço de **Transferências** atua como Orquestrador, utilizando comandos e eventos no Kafka para coordenar o **débito** e **crédito** nos serviços de Contas Correntes.
* As operações são **Idempotentes** para lidar com *retries* de forma segura, característica essencial em sistemas distribuídos e resilientes (**Polly**).

### 🧱 Estrutura de Módulos

| Módulo | Camada / Tipo | Responsabilidade Principal |
| :--- | :--- | :--- |
| **`BankMore.Domain.*`** | Domínio (Core) | Regras de Negócio e Entidades. |
| **`BankMore.Application.*`** | Aplicação (CQRS) | **Commands** e **Queries** (via MediatR), Validação. |
| **`BankMore.Infra.Data.*`** | Infraestrutura (Persistência) | Implementação de **Repositórios** e **Unit Of Work** (Entity Framework Core). |
| **`BankMore.Infra.Kafka`** | Infraestrutura (Mensageria) | Implementação do **Outbox Pattern** e consumidores Kafka. |
| **`BankMore.Services.Api.*`** | Apresentação | Endpoints, Configuração e **Autenticação JWT**. |

---

💻 Techical Stack e Dependencies
--------------------------------

| **Categoria** | **Tecnologia** | **Padrão** | **Objetivo** |
| --- | --- | --- | --- |
| **Plataforma** | ASP.NET Core 8.0, .NET 8.0 |  | Base robusta para APIs Web. |
| **Segurança** | **ASP.NET Identity Core**, JWT | **Roles & Policies** | **Gerenciamento de usuários, geração de tokens e controle de acesso baseado em permissão.** |
| **Arquitetura** | MediatR, FluentValidation | **CQRS**, **Mediator** | Gestão do fluxo de Commands/Queries. |
| **Mensageria** | Apache Kafka, Kaflow |  | Publicação e consumo de eventos/comandos de forma distribuída. |
| **Persistência** | Entity Framework Core, MSSQL, Dapper | ORM, Migration | Acesso a dados e inicialização de schema. |
| **Padrões Avançados** | SAGA, Idempotência |  | Garantia de transações distribuídas e segurança de repetição. |
| **Consistência** | **Outbox Pattern** | **Transacional** | **Garante a atomicidade entre a persistência no banco de dados local e a publicação de mensagens no Kafka.** |
| **APIs** | **ASP.NET API Versioning** | **URL Routing** | **Permite evoluir a API sem quebrar clientes legados (ex: v1.0, v2.0).** |

---

## 📋 Requisitos de Negócio e Mapeamento de Controladores
Compreendido! Você deseja a documentação completa dos **três controllers** (`TransferenciaController`, `AccountController` e `ContaCorrenteController`), apresentados separadamente.

Aqui está a documentação detalhada para cada um.

* * * * *

1\. 📄 Documentação da API de Conta Corrente (ContaCorrenteController)
----------------------------------------------------------------------

**Path Base:** `/api/v1/ContaCorrente`

Este controller gerencia as operações de consulta e gerenciamento de Contas Correntes.

### 🔍 Endpoints de Consulta (GET)

Todos os GETs utilizam a **Policy `OwnerOrMaster_Conta`**, que permite acesso à **própria conta** (via token) ou a **contas de terceiros** se o usuário for **Master/Admin**.

| **Método** | **Endpoint** | **Ação / Serviço** | **Descrição** | **Respostas (Sucesso)** |
| --- | --- | --- | --- | --- |
| **GET** | `/informacoes` | `_contaCorrenteService` | Consulta as **informações básicas** (Nome, Número, Status) da conta. | **200 OK** (`InformacoesContaCorrenteDto`) |
| **GET** | `/saldo` | `_saldoService` | Consulta o **saldo atual** e totais de crédito/débito. | **200 OK** (`SaldoDto`) |
| **GET** | `/extrato` | `_extratoService` | Gera o **extrato** completo de movimentações (débitos e créditos) por período. | **200 OK** (`IEnumerable<ExtratoDto>`) |

### ✍️ Endpoints de Escrita (POST e PUT)

| **Método** | **Endpoint** | **Ação / Serviço** | **Descrição** | **Policy de Acesso** |
| --- | --- | --- | --- | --- |
| **POST** | `/` | `_contaCorrenteService` | **Cadastra uma nova conta corrente** no sistema. | **`CanWriteDataOrMasterPolicy`** |
| **PUT** | `/` | `_contaCorrenteService` | **Altera os dados** de uma conta existente. Uso **administrativo**. | **`CanWriteData`** e **Role `Master` ou `Admin`** |

* * * * *

2\. 📄 Documentação da API de Transferências (TransferenciaController)
----------------------------------------------------------------------

**Path Base:** `/api/v1/Transferencia`

Este controller é o ponto de entrada para iniciar novas transferências entre contas.

### 💸 Endpoint de Criação de Transferência

| **Método** | **Endpoint** | **Ação / Serviço** | **Descrição** | **Padrão de Comunicação** |
| --- | --- | --- | --- | --- |
| **POST** | `/` | `_transferenciasService.Cadastrar(viewModel)` | **Realiza uma nova transferência**. Esta é uma **operação assíncrona** que **inicia a SAGA de transferência** (coreografada via Kafka). O resultado final da movimentação é processado externamente. | **Assíncrona (SAGA via Kafka)** |

#### **Segurança e Detalhe Chave**

-   **Segurança:** Requer a Policy **`CanWriteDataOrMasterPolicy`**.

-   **Fluxo:** O controller apenas submete o comando. Se o registro for bem-sucedido, o processo de débito/crédito ocorre em serviços de *background*.

* * * * *

3\. 📄 Documentação da API de Identidade (AccountController)
------------------------------------------------------------

**Path Base:** O controller é acessado via rotas diretas, como `/login`, `/register`, etc.

Este controller lida com a autenticação, registro de usuários e gestão de tokens JWT.

### 🔑 Endpoints de Autenticação e Token

| **Método** | **Endpoint** | **Ação / Serviço** | **Descrição** | **Acesso** |
| --- | --- | --- | --- | --- |
| **POST** | `/login` | `UserManager`, `IJwtFactory` | **Autentica o usuário** (CPF e Senha) e gera um par de **Access Token** (JWT) e **Refresh Token**. A senha é validada manualmente contra o hash com salt implícito (ID do usuário). | **Anônimo (`[AllowAnonymous]`)** |
| **POST** | `/refresh` | `AuthDbContext`, `IJwtFactory` | **Renova o Access Token** usando um **Refresh Token** válido. O token antigo é marcado como `Used` e um novo par é emitido. | **Anônimo (`[AllowAnonymous]`)** |

### 📝 Endpoints de Gestão de Usuário e Roles

| **Método** | **Endpoint** | **Ação / Serviço** | **Descrição** | **Policy de Acesso** |
| --- | --- | --- | --- | --- |
| **POST** | `/register` | `UserManager` + `_contaCorrenteService` | **Registra um novo usuário** e **dispara um evento para o Kafka** (`UsuarioCriadoEvent`) para provisionar a conta corrente. | **Anônimo (`[AllowAnonymous]`)** |
| **GET** | `/current` | `IUser` injetado | Retorna o status de autenticação e todas as **Claims** (incluindo roles e `numero_conta`) do usuário autenticado. | **Autorizado (`[Authorize]`)** |
| **POST** | `/update-role` | `UserManager` e `RoleManager` | **Atribui uma nova Role** a um usuário existente (por CPF). | **`MasterAccess`** |

#### **Comunicação Assíncrona no Registro**

O `POST /register` demonstra o uso do **Outbox Pattern** ou comunicação assíncrona: o sucesso no registro do usuário **depende** do sucesso no envio do evento para o Kafka (`_contaCorrenteService.CadastrarConta(evento)`). Se o envio falhar, o usuário é deletado do Identity.

* * * * *

### 🔑 Padrões de Segurança e Comunicação Chave

| **Feature** | **Descrição e Contexto** |
| --- | --- |
| **Autorização Centralizada** | Todos os endpoints herdam de `ApiController` e utilizam o **JWT** (JSON Web Token) para autenticação. |
| **Policy `OwnerOrMaster_Conta`** | Implementa uma lógica de **acesso contextual**: permite acesso total a `Master/Admin` ou apenas à **própria conta** para usuários comuns (o número da conta é extraído do `Claim` do token via `IUser`). |
| **Policy `CanWriteDataOrMasterPolicy`** | Controla o acesso à criação de recursos, geralmente permitindo usuários com permissão de escrita OU *Role* **Master/Admin**. |
| **Model State Validation** | O método `NotifyModelStateErrors()` (herdados de `ApiController`) é usado para lidar com erros de validação do **`[FromBody]`** em requisições **POST** e **PUT**. |

### 2. API Conta Corrente (`BankMore.Services.Api.ContasCorrentes`)

| Controller / Endpoint | Ação / Serviço | Padrão de Segurança / Comunicação |
| :--- | :--- | :--- |
| **`ContaCorrenteController.Get*`** | Usa `_contaCorrenteService`, `_saldoService`, `_extratoService`. | **Autorização por Policy** (`OwnerOrMaster_Conta`), garantindo que o usuário só acesse a própria conta (via `IUser` injetado) ou contas de terceiros se for Master/Admin. |
| **`MovimentoController.PostCadastrar`** | A ação de movimentação é delegada ao **`_movimentarKafkaService`**. | **Comunicação Assíncrona (Kafka):** O controller apenas submete o comando, que é processado por um consumidor (Worker) de forma assíncrona. |
| **`ContaCorrenteController.PostCadastrar`** | Cria uma nova conta. | **Autorização por Policy** (`CanWriteDataOrMasterPolicy`). |

### 3. API Transferência (`BankMore.Services.Api.Transferencias`)

| Controller / Endpoint | Ação / Serviço | Padrão de Comunicação |
| :--- | :--- | :--- |
| **`TransferenciaController.PostCadastrar`** | Usa `_transferenciasService.Cadastrar` (Método assíncrono). | **Início da SAGA:** Este é o ponto inicial da transação distribuída, que, internamente, usa o **Outbox/Kafka** para coordenar o fluxo de débito/crédito. |

---

🚀 Como Rodar o Projeto (Local)
-------------------------------

O projeto é configurado para ser executado completamente via **Docker Compose**, inicializando a stack de infraestrutura (**MSSQL**, **Kafka**) e todas as **APIs**.

* * * * *

### 1\. Limpeza e Remoção de Artefatos Antigos

Execute este comando para garantir um ambiente limpo:

Bash

```
docker compose -f .\docker-compose.yml -f .\docker-compose.development.yml down --volumes --remove-orphans

```

* * * * *

### 2\. Inicialização da Infraestrutura e APIs (Modo Desenvolvimento)

Inicia o **SQL Server** (`mssql`, `mssql-init`), a stack **Kafka** e todas as **APIs** em modo detached (`-d`).

Bash

```
docker compose -f .\docker-compose.yml -f .\docker-compose.development.yml up -d mssql mssql-init zookeeper broker schema-registry kafka-tools kafka-ui

```

* * * * *

### 3\. Inspeção e Debug (Modo Produção/Forçar Build)

Use este comando para **reconstruir** as imagens, **forçar a recriação** e visualizar o progresso dos logs.

Bash

```
docker compose --progress plain -f .\docker-compose.yml -f .\docker-compose.production.yml up --build --no-deps --force-recreate

```

* * * * *

🔎 Comandos Kafka Úteis
-----------------------

Estes comandos devem ser executados **dentro do container broker** para inspeção e monitoramento:

| **Ação** | **Comando** |
| --- | --- |
| **Listar Tópicos** | `docker exec -it broker /bin/bash -c "kafka-topics --bootstrap-server broker:29092 --list"` |
| **Listar Consumers** | `docker exec -it broker kafka-consumer-groups --bootstrap-server localhost:9092 --list` |
| **Descrever Consumer Group** | `docker exec -it broker kafka-consumer-groups --bootstrap-server localhost:9092 --group grupo.aplicacao.transferencia.saga.orquestrador --describe` |
| **Listar Mensagens (Exemplo)** | `docker exec -it broker kafka-console-consumer --bootstrap-server localhost:9092 --topic saga.movimentar.conta.cmd --from-beginning --max-messages 10` |

* * * * *

📊 Endpoints de Verificação
---------------------------

### 📖 Swagger UI (Apenas ambiente Dev)

`http://localhost:5001/swagger`

### ❤️ Health Check (Ambientes Staging & Prod)

`http://localhost:5001/hc-ui`

* * * * *


O FLUXO DE NAVEGAÇÃO COMPLETO NO KUBERNETES
===========================================

O processo de uma requisição externa, como acessar `http://[IP_PÚBLICO]/identidade/swagger`, envolve a coordenação de quatro componentes principais do Kubernetes: o **Load Balancer**, o **Ingress Controller Service**, o **Recurso Ingress** (a regra) e o **Service ClusterIP** de destino.

O Processo de 6 Etapas
----------------------

O tráfego segue uma jornada bem definida no cluster:

### 1\. Origem: O Cliente e o DNS

O cliente (navegador ou outra aplicação) tenta acessar o endpoint. Ele resolve o nome de domínio para o **IP Público** (Load Balancer IP) que o seu Ingress Controller expõe.

### 2\. Ponto de Entrada: O Balanceador de Carga (Load Balancer)

O tráfego HTTP/HTTPS chega primeiro ao Load Balancer do provedor de cloud (Azure, AWS, GKE, etc.). Este Load Balancer, por sua vez, foi provisionado e configurado automaticamente pelo Kubernetes por causa do **Service do NGINX Ingress Controller** ser do tipo `LoadBalancer`.

-   **Função:** A única função neste ponto é encaminhar todo o tráfego da porta 80 e 443 para as portas correspondentes dos nós de trabalho (Worker Nodes) que hospedam o Ingress Controller.

### 3\. A Ponte Interna: O Service do Ingress Controller

O tráfego do Load Balancer chega ao Service do Ingress Controller (ex: `ingress-nginx-controller`) que roda no namespace `ingress-nginx`.

-   **Função:** Ele atua como uma porta de entrada do cluster, roteando o tráfego recebido para os Pods do Ingress Controller (onde o software NGINX está de fato rodando).

### 4\. O Roteador Central: O Pod do Ingress Controller (NGINX)

A requisição finalmente chega a um dos Pods onde o NGINX Ingress Controller está em execução. Este software é a inteligência do roteamento.

-   **Função:** O NGINX lê o **Recurso Ingress** que você criou (`bankmore-api-ingress`) e decide para onde a requisição deve ir.

### 5\. A Decisão: O Recurso Ingress (Regra)

O NGINX usa o `bankmore-api-ingress` como mapa. Na sua configuração final, a regra é simples:

```
paths:
  - path: /identidade
    pathType: Prefix
    backend:
      service:
        name: bankmore-identidade-svc
        port:
          number: 5000

```

-   **Ação:** O NGINX verifica o caminho da URL (`/identidade/swagger/v1/swagger.json`).

-   **Decisão (Graças ao `Prefix`):** Como o caminho começa com `/identidade`, o NGINX simplesmente encaminha a URL **completa** (`/identidade/swagger/v1/swagger.json`) para o Service interno de destino: `bankmore-identidade-svc` na porta 5000.

    -   *Nota:* As anotações de reescrita complexas (`rewrite-target: /$2`) teriam removido o prefixo `/identidade` aqui, mas como elas foram bloqueadas pelo seu cluster, o trabalho de remoção do prefixo passa para o microsserviço (próxima etapa).

### 6\. Destino Final: O Service ClusterIP e o Pod de Aplicação

A requisição agora está dentro do cluster, endereçada ao Service `bankmore-identidade-svc`.

-   **Função do Service:** O Service ClusterIP (porta 5000) atua como um load balancer interno, roteando a requisição para um dos Pods disponíveis que possuem o Label `app: bankmore-identidade`.

-   **Ação do Pod (O Segredo do C#):** A requisição chega ao seu microsserviço na porta 5000 com o caminho **completo** (`/identidade/swagger/v1/swagger.json`). É aqui que a configuração C# entra em ação:

    -   O método **`app.UsePathBase("/identidade")`** remove o prefixo `/identidade` da URL antes que ela seja processada pelo roteador da aplicação (Swagger/Minimal API).

    -   A aplicação vê apenas `/swagger/v1/swagger.json` e a processa corretamente, retornando a resposta.

Em resumo, o **Ingress (com `pathType: Prefix`)** roteia a requisição para o Service correto, e o **microsserviço C# (`UsePathBase`)** lida com o prefixo que foi mantido.

    > Frameworks padrão do **.NET** para segurança, autenticação de usuários e geração/validação de tokens de acesso com claims (roles/policies).
transações distribuídas em microsserviços.Outbox Pattern:Chris Richardson. Pattern: Outbox. Garante que a publicação de eventos seja atômica com a transação local do banco de dados.Domain-Driven Design (DDD):Eric Evans. Domain-Driven Design: Tackling Complexity in the Heart of Software. Foco na modelagem em torno do domínio de negócio.ASP.NET Identity Core & JWT:Frameworks padrão do .NET para segurança, autenticação de 
usuários e geração/validação de tokens de acesso com claims (roles/policies).


---

Guia de Comandos do Projeto BankMore (Formato Simples)
======================================================

Esta documentação resume os comandos mais utilizados no ciclo de vida do projeto, organizados por ambiente.

I. Gerenciamento do Ambiente Local (Docker Compose)
---------------------------------------------------

Estes comandos controlam a infraestrutura de desenvolvimento (SQL Server, Kafka, etc.).

-   **Destruição Completa (Cleanup):** Remove containers, volumes de dados persistentes e órfãos.

    ```
    docker compose -f .\docker-compose.yml -f .\docker-compose.development.yml down --volumes --remove-orphans

    ```

-   **Inicialização da Infra:** Levanta serviços essenciais (`mssql`, `zookeeper`, `broker`, etc.) em modo *detached*.

    ```
    docker compose -f .\docker-compose.yml -f .\docker-compose.development.yml up -d mssql mssql-init zookeeper broker schema-registry kafka-tools

    ```

-   **Execução/Reconstrução (Produção):** Levanta todos os serviços, forçando a reconstrução de imagens (`--build`).

    ```
    docker compose --progress plain -f .\docker-compose.yml -f .\docker-compose.production.yml up --build --no-deps --force-recreate

    ```

II. Construção e Publicação de Imagens Docker
---------------------------------------------

Passos para preparar e enviar as imagens das APIs .NET. (Substitua `seusuario` pelo seu Docker Hub username).

-   **Construir Imagem (.NET Identidade):**

    ```
    docker build -t phillrog/bankmore-api-identidade:latest -f Src/BankMore.Services.Api.Identidade/Dockerfile .

    ```

-   **Publicar (Push) a Imagem:**

    ```
    docker push phillrog/bankmore-api-identidade:latest

    ```

III. Gerenciamento e Monitoramento do Kafka
-------------------------------------------

Comandos para interagir diretamente com o Broker Kafka local.

-   **Listar Todos os Tópicos:**

    ```
    docker exec -it broker /bin/bash -c "kafka-topics --bootstrap-server broker:29092 --list"

    ```

-   **Listar Grupos de Consumidores Ativos:**

    ```
    docker exec -it broker kafka-consumer-groups --bootstrap-server localhost:9092 --list

    ```

-   **Verificar Detalhes/LAG de um Consumidor:** (Substitua `[GRUPO_ID]`)

    ```
    docker exec -it kafka kafka-consumer-groups --bootstrap-server localhost:9092 --group [GRUPO_ID] --describe

    ```

-   **Visualizar Primeiras Mensagens de um Tópico:** (Substitua `[TOPICO_ID]`)

    ```
    docker exec -it broker kafka-console-consumer --bootstrap-server localhost:9092 --topic [TOPICO_ID] --from-beginning --max-messages 10

    ```

IV. Gerenciamento de Infraestrutura (Terraform e Azure)
-------------------------------------------------------

Comandos para provisionar e remover a infraestrutura como código (IaS) na Azure.

```
# Instalar ferramentas (az cli, kubectl, sqlcmd, helm, etc...)

cd Deployment\Azure

./setup_vm.sh 
```

```
cd Deployment\Azure\terraform
```

-   **Configuração Inicial da VM:**

    ```
    chmod +x setup_vm.sh && ./setup_vm.sh

    ```

-   **Inicialização do Terraform:**

    ```
    terraform init

    ```

-   **Planejamento (Verificar Ações):**

    ```
    terraform plan

    ```

-   **Aplicação (Provisionar Recursos):**

    ```
    terraform apply -auto-approve

    ```

-   **DESTRUIÇÃO TOTAL DA INFRAESTRUTURA:** **(Operação irreversível!)**

    ```
    terraform destroy -auto-approve

    ```

-   **Obter Connection Strings SQL:**

    ```
    terraform output -raw sql_connection_strings_all

    ```

V. Gerenciamento do Cluster Kubernetes (K8s)
--------------------------------------------

Comandos essenciais para o cluster K8s.

-   **Instalar NGINX Ingress Controller:**

    ```
    kubectl create namespace ingress-nginx
    helm install ingress-nginx ingress-nginx/ingress-nginx --repo [https://kubernetes.github.io/ingress-nginx](https://kubernetes.github.io/ingress-nginx) --namespace ingress-nginx

    ```

-   **Verificar Endereço Público (LoadBalancer):**

    ```
    kubectl get svc -n ingress-nginx

    ```

-   **Reiniciar Todos os Deployments das APIs:**

    ```
    # Exemplo: Adapte a lista de deployments conforme necessário
    for deployment in identidade-deploy conta-corrente-deploy ; do kubectl rollout restart deployment $deployment -n kafka; done

    ```

-   **Monitorar Logs em Tempo Real (Exemplo Identidade):**

    ```
    POD_ID=$(kubectl get pods -n kafka | grep identidade-deploy | awk '{print $1}')
    kubectl logs -f $POD_ID -n kafka


Endereços das APIs (Via K8s Ingress)
====================================

**IMPORTANTE:** Substitua `http://[IP_PÚBLICO_DO_NGINX]` pelo IP real do seu Load Balancer NGINX.


Documentação (Swagger UI)
-------------------------

-   **API de Identidade:** `http://[IP_PÚBLICO_DO_NGINX]/identidade/swagger/index.html`

-   **API de Contas Correntes:** `http://[IP_PÚBLICO_DO_NGINX]/contascorrentes/swagger/index.html`

-   **API de Transferências:** `http://[IP_PÚBLICO_DO_NGINX]/transferencias/swagger/index.html`

Endereços Base (Endpoints)
--------------------------

-   **API de Identidade (Base):** `http://[IP_PÚBLICO_DO_NGINX]/identidade`

-   **API de Contas Correntes (Base):** `http://[IP_PÚBLICO_DO_NGINX]/contascorrentes`

-   **API de Transferências (Base):** `http://[IP_PÚBLICO_DO_NGINX]/transferencias`

Atenção habilitar a porta do ingress porta 80
---    

# Resultado Final

# Apis

## Api.Identidade

![cadastro](https://github.com/user-attachments/assets/5e7b33ea-b606-4c07-8063-291e378c0ed0)

## Api.ContasCorresntes

![movimentacao](https://github.com/user-attachments/assets/a03f0346-45e2-41d7-913d-4b7daae293e1)

## Api.Transferencias

![transferencia](https://github.com/user-attachments/assets/395fbc79-3f2d-4de6-b1e5-516ceb27f68f)

## Extrato

![extrato](https://github.com/user-attachments/assets/e9279efb-99d9-49f6-8484-d14e1c0f8e31)




<img width="1907" height="971" alt="image" src="https://github.com/user-attachments/assets/6fcf6bd2-8269-44e3-b5c2-4e7fc0a48c55" />


Banco de dados

<img width="1912" height="763" alt="image" src="https://github.com/user-attachments/assets/e627c44e-9220-4f3a-848e-4bb4c946a104" />


<img width="1919" height="971" alt="Captura de tela 2025-11-23 183447" src="https://github.com/user-attachments/assets/b102006b-3222-4d44-8c63-f4e8e9747235" />

<img width="1919" height="969" alt="Captura de tela 2025-11-23 183455" src="https://github.com/user-attachments/assets/2a21a5a1-afcf-44e4-9f85-bbfea9f9bd0d" />

<img width="1919" height="978" alt="Captura de tela 2025-11-23 183526" src="https://github.com/user-attachments/assets/a410dad9-10a2-4f11-b9b6-8c588d19176d" />


Cluster k8s

<img width="1917" height="967" alt="image" src="https://github.com/user-attachments/assets/23ecd1c8-a3fd-4075-9888-7fbc32e2d821" />

Deployments

<img width="1917" height="623" alt="image" src="https://github.com/user-attachments/assets/78fad580-295e-47e5-9095-a652176b46c4" />

<img width="1600" height="954" alt="Captura de tela 2025-11-23 190608" src="https://github.com/user-attachments/assets/c1ad6bf7-3613-453a-8803-57745ffc8b9e" />




# Kafka ui

<img width="1118" height="226" alt="image" src="https://github.com/user-attachments/assets/1e231c12-bb39-4c7d-9225-40431860e836" />


<img width="1917" height="691" alt="image" src="https://github.com/user-attachments/assets/e5f9aa34-d4fc-41ca-be1c-f19932d69728" />

<img width="1913" height="807" alt="image" src="https://github.com/user-attachments/assets/124c1f94-1484-4d12-99c6-4b8e7d272388" />



📚 Referências e Conceitos Chave
--------------------------------

-   **SAGA:** `https://martinfowler.com/articles/microservices.html#saga`

-   **Outbox:** `https://microservices.io/patterns/data/transactional-outbox.html`

-   **DDD (Livro):** `https://www.amazon.com.br/Domain-Driven-Design-Tackling-Complexity-Software/dp/0321125215`

-   **KafkaFlow:** `https://kafkaflow.io/docs/getting-started/introduction`

-   **MediatR:** `https://github.com/jbogard/MediatR`

-   **Kafka:** `https://www.confluent.io`

-   **Kafka:** `https://www.confluent.io`

-   **Terraform:** `https://developer.hashicorp.com/terraform/docs`

-   **Azure DevOps:** `https://learn.microsoft.com/pt-br/azure/devops/?view=azure-devops`
