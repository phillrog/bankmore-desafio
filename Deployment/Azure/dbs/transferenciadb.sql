-- Este script contém apenas comandos DDL/DML para o BankMoreTransferenciaDB.
-- O banco de dados foi criado pelo Terraform, e o contexto é definido pelo sqlcmd (parâmetro -d).

PRINT 'Iniciando criação de esquema para Transferencia DB...';
GO

-- 1. Criação das Tabelas do TransferenciaDB

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- transferencia
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[transferencia]') AND type in (N'U'))
BEGIN
    CREATE TABLE transferencia (
        id UNIQUEIDENTIFIER PRIMARY KEY,
        idcontacorrenteorigem UNIQUEIDENTIFIER NOT NULL,
        idcontacorrentedestino UNIQUEIDENTIFIER NOT NULL,
        datamovimento DATETIME2 NOT NULL,
        valor DECIMAL(10, 2) NOT NULL,
        status INTEGER NOT NULL,
        dataultimaalteracao DATETIME2 NULL,
        erro VARCHAR(MAX),
        [CreatedAt] [datetime2](7) NOT NULL,
        [CreatedBy] INT NOT NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        [UpdatedBy] INT NOT NULL,
        [IsDeleted] BIT NOT NULL
    );
END
GO

-- idempotencia
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[idempotencia]') AND type in (N'U'))
BEGIN
    CREATE TABLE idempotencia (
        id UNIQUEIDENTIFIER PRIMARY KEY,     
        idcontacorrente UNIQUEIDENTIFIER NOT NULL,
        requisicao NVARCHAR(MAX), 
        resultado NVARCHAR(MAX),
        [CreatedAt] [datetime2](7) NOT NULL,
        [CreatedBy] INT NOT NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        [UpdatedBy] INT NOT NULL,
        [IsDeleted] BIT NOT NULL,  
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

PRINT 'Criação do esquema para Transferencia DB concluída.';
GO