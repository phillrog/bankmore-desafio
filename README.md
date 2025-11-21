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
docker compose -f .\docker-compose.yml -f .\docker-compose.development.yml up -d mssql mssql-init zookeeper broker schema-registry kafka-tools

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

📚 Referências e Conceitos Chave
--------------------------------

-   **SAGA Pattern:**

    > Martin Fowler. Saga. Abordagem para gerenciar transações distribuídas em microsserviços.

-   **Outbox Pattern:**

    > Chris Richardson. Pattern: Outbox. Garante que a publicação de eventos seja atômica com a transação local do banco de dados.

-   **Domain-Driven Design (DDD):**

    > Eric Evans. Domain-Driven Design: Tackling Complexity in the Heart of Software. Foco na modelagem em torno do domínio de negócio.

-   **ASP.NET Identity Core & JWT:**

    > Frameworks padrão do **.NET** para segurança, autenticação de usuários e geração/validação de tokens de acesso com claims (roles/policies).
transações distribuídas em microsserviços.Outbox Pattern:Chris Richardson. Pattern: Outbox. Garante que a publicação de eventos seja atômica com a transação local do banco de dados.Domain-Driven Design (DDD):Eric Evans. Domain-Driven Design: Tackling Complexity in the Heart of Software. Foco na modelagem em torno do domínio de negócio.ASP.NET Identity Core & JWT:Frameworks padrão do .NET para segurança, autenticação de 
usuários e geração/validação de tokens de acesso com claims (roles/policies).
