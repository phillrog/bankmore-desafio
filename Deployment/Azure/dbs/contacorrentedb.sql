-- Este script contém apenas comandos DDL/DML para o BankMoreContaCorrenteDB.
-- O banco de dados foi criado pelo Terraform, e o contexto é definido pelo sqlcmd (parâmetro -d).

PRINT 'Iniciando criação de esquema e seed de dados para Conta Corrente DB...';
GO

-- 1. Criação das Tabelas do ContaCorrenteDB

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- contacorrente
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[contacorrente]') AND type in (N'U'))
BEGIN
    CREATE TABLE contacorrente (
        id UNIQUEIDENTIFIER PRIMARY KEY,
        numero INTEGER NOT NULL UNIQUE,
        nome VARCHAR(100) NOT NULL,
        ativo BIT NOT NULL default 1,
        senha VARCHAR(100) NOT NULL,
        salt VARCHAR(100),
        cpf VARCHAR(20),
        [CreatedAt] [datetime2](7) NOT NULL,
        [CreatedBy] INT NOT NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        [UpdatedBy] INT NOT NULL,
        [IsDeleted] BIT NOT NULL
    )
END
GO

-- movimento
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[movimento]') AND type in (N'U'))
BEGIN
    CREATE TABLE movimento (
        id UNIQUEIDENTIFIER PRIMARY KEY,
        idcontacorrente UNIQUEIDENTIFIER NOT NULL,
        datamovimento DATETIME2(0) NOT NULL,
        tipomovimento CHAR(1) NOT NULL,
        valor DECIMAL(18, 2) NOT NULL,
        CreatedAt DATETIME2(7) NOT NULL,
        CreatedBy INT NOT NULL,
        UpdatedAt DATETIME2(7) NOT NULL,
        UpdatedBy INT NOT NULL,
        IsDeleted BIT NOT NULL,
        idtransferencia UNIQUEIDENTIFIER NULL,
        -- Chave Estrangeira: Assumindo que o DB Contas Correntes só precisa da referência local.
        FOREIGN KEY (idcontacorrente) REFERENCES contacorrente(id) 
    );
END
GO

-- idempotencia
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[idempotencia]') AND type in (N'U'))
BEGIN
    CREATE TABLE idempotencia (
        id UNIQUEIDENTIFIER PRIMARY KEY,
        idcontacorrente UNIQUEIDENTIFIER NULL,
        requisicao NVARCHAR(MAX) NULL,
        resultado NVARCHAR(MAX) NULL,
        idtransferencia UNIQUEIDENTIFIER NULL,
        CreatedAt DATETIME2(7) NOT NULL,
        CreatedBy INT NOT NULL,
        UpdatedAt DATETIME2(7) NOT NULL,
        UpdatedBy INT NOT NULL,
        IsDeleted BIT NOT NULL,
        FOREIGN KEY (idcontacorrente) REFERENCES contacorrente(id)
    );
END
GO

-- outboxMessages
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[outboxMessages]') AND type in (N'U'))
BEGIN
    CREATE TABLE outboxMessages (
        Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,     
        CreatedOn DATETIME2(7) NOT NULL,   
        Type NVARCHAR(255) NOT NULL,
        Payload NVARCHAR(MAX) NOT NULL,
        IsProcessed BIT NOT NULL DEFAULT 0,
        CorrelationId UNIQUEIDENTIFIER NULL 
    );
END
GO

-- Criação de Índices
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Outbox_Pending' AND object_id = OBJECT_ID('outboxMessages'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Outbox_Pending 
    ON OutboxMessages (IsProcessed, CreatedOn)
    WHERE IsProcessed = 0;
END
GO

-- 2. Seed de Conta Master (ContaCorrenteDB)

DECLARE @CurrentDate DATETIME2(7) = GETDATE();
DECLARE @MasterPasswordHash NVARCHAR(MAX) = N'10000.YXwK4ASk1S6XvxAGMEDEIw==.IbEYeANMiR1CsNftByIP0flRlAtxGUP4uns7c6o6lCI=';
DECLARE @MasterId UNIQUEIDENTIFIER = 'c40cb0c3-d8d2-474d-b172-21785d5f71f8'; 
DECLARE @MasterCpf NVARCHAR(20) = N'00000000000';
DECLARE @MasterNome NVARCHAR(256) = N'MASTER';

PRINT 'Iniciando Seed de Usuário Master no Conta Corrente DB...';

IF NOT EXISTS (SELECT 1 FROM [dbo].[contacorrente] WHERE id = @MasterId)
BEGIN
    INSERT INTO [dbo].[contacorrente]
    (
        id, numero, nome, ativo, senha, salt, cpf,
        [CreatedAt], [CreatedBy], [UpdatedAt], [UpdatedBy], [IsDeleted]
    )
    VALUES
    (
        @MasterId,
        -1, 
        @MasterNome,
        1, 
        @MasterPasswordHash,
        @MasterPasswordHash,
        @MasterCpf,
        @CurrentDate,
        0,
        @CurrentDate,
        0,
        0 
    );
    PRINT 'Usuário Master (Conta Corrente) inserido.';
END
ELSE
BEGIN
    PRINT 'O usuário Master (Conta Corrente) já existe. Pulando a inserção.';
END
GO