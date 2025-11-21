USE [master]
GO
/****** Object:  Database [BankMoreIdentidadeDB]    Script Date: 15/11/2025 10:00:46 ******/
CREATE DATABASE [BankMoreIdentidadeDB]
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BankMoreIdentidadeDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET ANSI_NULLS OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET ANSI_PADDING OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET ARITHABORT OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET  ENABLE_BROKER
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET ALLOW_SNAPSHOT_ISOLATION ON
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET READ_COMMITTED_SNAPSHOT ON
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET RECOVERY FULL
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET  MULTI_USER
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET DB_CHAINING OFF
GO

USE [BankMoreIdentidadeDB]
GO
/****** Object:  Table [dbo].[AspNetRoleClaims]    Script Date: 15/11/2025 10:00:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoleClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 15/11/2025 10:00:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](450) NOT NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[Name] [nvarchar](256) NULL,
	[NormalizedName] [nvarchar](256) NULL,
 CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 15/11/2025 10:00:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 15/11/2025 10:00:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](450) NOT NULL,
	[ProviderKey] [nvarchar](450) NOT NULL,
	[ProviderDisplayName] [nvarchar](max) NULL,
	[UserId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED
(
	[LoginProvider] ASC,
	[ProviderKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 15/11/2025 10:00:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](450) NOT NULL,
	[RoleId] [nvarchar](450) NOT NULL,
 CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 15/11/2025 10:00:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](450) NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[ConcurrencyStamp] [nvarchar](max) NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[LockoutEnd] [datetimeoffset](7) NULL,
	[NormalizedEmail] [nvarchar](256) NULL,
	[NormalizedUserName] [nvarchar](256) NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[UserName] [nvarchar](256) NULL,
	[Cpf] [nvarchar](20) NULL,
 CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[AspNetUserTokens]    Script Date: 15/11/2025 10:00:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserTokens](
	[UserId] [nvarchar](450) NOT NULL,
	[LoginProvider] [nvarchar](450) NOT NULL,
	[Name] [nvarchar](450) NOT NULL,
	[Value] [nvarchar](max) NULL,
 CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED
(
	[UserId] ASC,
	[LoginProvider] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

GO
/****** Object:  Table [dbo].[Customers]    Script Date: 15/11/2025 10:00:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Customers](
	[Id] [uniqueidentifier] NOT NULL,
	[BirthDate] [datetime2](7) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[Name] [varchar](100) NOT NULL,
    [CreatedAt] [datetime2](7) NOT NULL,
    [CreatedBy] INT NOT NULL,
    [UpdatedAt] [datetime2](7) NOT NULL,
    [UpdatedBy] INT NOT NULL,
    [IsDeleted] BIT NOT NULL,
 CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)

CREATE TABLE [dbo].[RefreshTokens] (
  [Token] nvarchar(max) NOT NULL,
  [JwtId] nvarchar(max) NULL,
  [CreationDate] datetime2(0) NULL,
  [ExpiryDate] datetime2(0) NULL,
  [Used] bit NULL,
  [Invalidated] bit NULL,
  [UserId] nvarchar(450) NULL
)
GO

INSERT INTO [dbo].[AspNetRoles]([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'4f7d42de-333e-43a7-afd8-09715f14a08c', N'e2ec2f11-acef-443c-9e38-2fd64ffd7b92', N'Admin', N'ADMIN');
INSERT INTO [dbo].[AspNetRoles]([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'14b9234c-e7e6-4cf7-8a7b-82467ecc6b6b', N'14b9234c-e7e6-4cf7-8a7b-82467ecc6b6b', N'Master', N'MASTER');

GO
SET ANSI_PADDING OFF
GO

/****** Object:  Index [IX_AspNetRoleClaims_RoleId]    Script Date: 15/11/2025 10:00:46 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId] ON [dbo].[AspNetRoleClaims]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [RoleNameIndex]    Script Date: 15/11/2025 10:00:46 ******/
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex] ON [dbo].[AspNetRoles]
(
	[NormalizedName] ASC
)
WHERE ([NormalizedName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AspNetUserClaims_UserId]    Script Date: 15/11/2025 10:00:46 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId] ON [dbo].[AspNetUserClaims]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AspNetUserLogins_UserId]    Script Date: 15/11/2025 10:00:46 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId] ON [dbo].[AspNetUserLogins]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_AspNetUserRoles_RoleId]    Script Date: 15/11/2025 10:00:46 ******/
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId] ON [dbo].[AspNetUserRoles]
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [EmailIndex]    Script Date: 15/11/2025 10:00:46 ******/
CREATE NONCLUSTERED INDEX [EmailIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedEmail] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UserNameIndex]    Script Date: 15/11/2025 10:00:46 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex] ON [dbo].[AspNetUsers]
(
	[NormalizedUserName] ASC
)
WHERE ([NormalizedUserName] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
ALTER TABLE [dbo].[AspNetRoleClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetRoleClaims] CHECK CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserClaims]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserClaims] CHECK CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserLogins]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserLogins] CHECK CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[AspNetRoles] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId]
GO
ALTER TABLE [dbo].[AspNetUserRoles]  WITH CHECK ADD  CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY([UserId])
REFERENCES [dbo].[AspNetUsers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[AspNetUserRoles] CHECK CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
GO
USE [master]
GO
ALTER DATABASE [BankMoreIdentidadeDB] SET  READ_WRITE
GO

----------------------------------------------------------------------------------------------
---- BANCO CONTACORRENTE
USE [master]
GO
/****** Object:  Database [BankMoreContaCorrenteDB]    Script Date: 15/11/2025 10:00:46 ******/
CREATE DATABASE [BankMoreContaCorrenteDB]
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BankMoreContaCorrenteDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET ANSI_NULLS OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET ANSI_PADDING OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET ARITHABORT OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET  ENABLE_BROKER
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET ALLOW_SNAPSHOT_ISOLATION ON
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET READ_COMMITTED_SNAPSHOT ON
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET RECOVERY FULL
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET  MULTI_USER
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [BankMoreContaCorrenteDB] SET DB_CHAINING OFF
GO

USE [BankMoreContaCorrenteDB]
GO


CREATE TABLE contacorrente (
	id UNIQUEIDENTIFIER PRIMARY KEY, -- id da conta corrente
	numero INTEGER NOT NULL UNIQUE, -- numero da conta corrente
	nome VARCHAR(100) NOT NULL, -- nome do titular da conta corrente
	ativo BIT NOT NULL default 1, -- indicativo se a conta esta ativa. (0 = inativa, 1 = ativa).
	senha VARCHAR(100) NOT NULL,
	salt VARCHAR(100),
	cpf VARCHAR(20),
    [CreatedAt] [datetime2](7) NOT NULL,
    [CreatedBy] INT NOT NULL,
    [UpdatedAt] [datetime2](7) NOT NULL,
    [UpdatedBy] INT NOT NULL,
    [IsDeleted] BIT NOT NULL
)
GO
CREATE TABLE movimento (
    id UNIQUEIDENTIFIER PRIMARY KEY, -- ID único do movimento
    idcontacorrente UNIQUEIDENTIFIER NOT NULL, -- ID da conta corrente (FK)
    datamovimento DATETIME2(0) NOT NULL, -- Data e hora do movimento
    tipomovimento CHAR(1) NOT NULL, -- Tipo do movimento (C ou D)
    valor DECIMAL(18, 2) NOT NULL, -- Valor do movimento (moeda)
    
    -- Colunas de Auditoria
    CreatedAt DATETIME2(7) NOT NULL,
    CreatedBy INT NOT NULL,
    UpdatedAt DATETIME2(7) NOT NULL,
    UpdatedBy INT NOT NULL,
    IsDeleted BIT NOT NULL,
    idtransferencia UNIQUEIDENTIFIER NULL, -- ID da transferencia quando tiver
    -- Definição da Chave Estrangeira
    -- Referencia a coluna 'id' na tabela 'contacorrente'
    FOREIGN KEY (idcontacorrente) REFERENCES contacorrente(id) 
);
GO
CREATE TABLE idempotencia (
    id UNIQUEIDENTIFIER PRIMARY KEY, -- Chave de idempotência (GUID)
    idcontacorrente UNIQUEIDENTIFIER NULL, -- ID da conta (Adicionado para suportar a FK)
    requisicao NVARCHAR(MAX) NULL, -- Dados de requisição (JSON/XML/String)
    resultado NVARCHAR(MAX) NULL, -- Dados de retorno (JSON/XML/String)
    idtransferencia UNIQUEIDENTIFIER NULL, -- ID da transferencia quando tiver
    -- Colunas de Auditoria
    CreatedAt DATETIME2(7) NOT NULL,
    CreatedBy INT NOT NULL,
    UpdatedAt DATETIME2(7) NOT NULL,
    UpdatedBy INT NOT NULL,
    IsDeleted BIT NOT NULL,
    
    -- Definição da Chave Estrangeira
    -- Referencia a coluna 'id' na tabela 'contacorrente'
    FOREIGN KEY (idcontacorrente) REFERENCES contacorrente(id)
);

CREATE TABLE outboxMessages (
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,     
    CreatedOn DATETIME2(7) NOT NULL,    
    Type NVARCHAR(255) NOT NULL,
    Payload NVARCHAR(MAX) NOT NULL,
    
    IsProcessed BIT NOT NULL DEFAULT 0,
  
    CorrelationId UNIQUEIDENTIFIER NULL 
);
GO

CREATE NONCLUSTERED INDEX IX_Outbox_Pending 
ON OutboxMessages (IsProcessed, CreatedOn)
WHERE IsProcessed = 0;
GO


-- =========================================================
-- CONFIGS
-- =========================================================
USE [BankMoreIdentidadeDB]
GO

DECLARE @CurrentDate DATETIME2(7) = GETDATE();
-- O hash é calculado como Hash(SenhaPura + ID_do_usuario_em_minusculas)
DECLARE @MasterPasswordHash NVARCHAR(MAX) = N'10000.YXwK4ASk1S6XvxAGMEDEIw==.IbEYeANMiR1CsNftByIP0flRlAtxGUP4uns7c6o6lCI='; 
-- O 'salt' será o ID do usuário, que é a parte que foi concatenada na senha para gerar o hash
DECLARE @MasterId NVARCHAR(100) = N'C40CB0C3-D8D2-474D-B172-21785D5F71F8';
DECLARE @MasterRoleId NVARCHAR(450) = (SELECT ID FROM [dbo].[AspNetRoles] WHERE NAME = 'Master'); -- Busca o id da role master
DECLARE @MasterLogin NVARCHAR(20) = N'00000000000';
DECLARE @MasterNormalizedUserName NVARCHAR(256) = (select @MasterLogin);

DECLARE @MasterCpf NVARCHAR(20) = (select @MasterLogin);
DECLARE @MasterNome NVARCHAR(256) = N'MASTER';
-- =========================================================
-- 1. BANCO BankMoreIdentidadeDB (Identity)
-- =========================================================


PRINT 'Iniciando Seed no BankMoreIdentidadeDB...';

-- Inserir o usuário Master na tabela AspNetUsers
IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetUsers] WHERE Id = @MasterId)
BEGIN
    INSERT INTO [dbo].[AspNetUsers]
    (
        [Id],
        [AccessFailedCount],
        [ConcurrencyStamp],
        [EmailConfirmed],
        [LockoutEnabled],
        [LockoutEnd],
        [PhoneNumberConfirmed],
        [TwoFactorEnabled],
        [UserName],
        [NormalizedUserName],
        [Cpf],
        [PasswordHash],
        [SecurityStamp]
    )
    VALUES
    (
        @MasterId,
        0,
        NEWID(), 
        1,       
        1,       
        NULL,
        0,       
        0,       
        @MasterLogin,
        @MasterNormalizedUserName,
        @MasterLogin,
        @MasterPasswordHash,
        NEWID()
    );

	-- Atribuir a Role 'Master' ao usuário Master    
    INSERT INTO [dbo].[AspNetUserRoles]
    (
        [UserId],
        [RoleId]
    )
    VALUES
    (
        @MasterId,
        @MasterRoleId
    );

    
    PRINT 'Usuário Master inserido e Role Master atribuída no IdentityDB.';
END
ELSE
BEGIN
    PRINT 'O usuário Master já existe no IdentityDB. Pulando a inserção.';
END
GO


-- =========================================================
-- 2. BANCO BankMoreContaCorrenteDB (Conta Corrente)
-- =========================================================
USE [BankMoreContaCorrenteDB]
GO

DECLARE @CurrentDate DATETIME2(7) = GETDATE();
-- O hash é calculado como Hash(SenhaPura + ID_do_usuario_em_minusculas)
DECLARE @MasterPasswordHash NVARCHAR(MAX) = N'10000.YXwK4ASk1S6XvxAGMEDEIw==.IbEYeANMiR1CsNftByIP0flRlAtxGUP4uns7c6o6lCI='; 
-- O 'salt' será o ID do usuário, que é a parte que foi concatenada na senha para gerar o hash
DECLARE @MasterId NVARCHAR(100) = N'c40cb0c3-d8d2-474d-b172-21785d5f71f8';
DECLARE @MasterCpf NVARCHAR(20) = N'00000000000';
DECLARE @MasterNome NVARCHAR(256) = N'MASTER';

PRINT 'Iniciando Seed no BankMoreContaCorrenteDB...';

-- Inserir o usuário Master na tabela contacorrente
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
        -1, --- CONTA PARA DEVS NÚMERO NEGATIVO PARA NÃO MISTURAR COM USUÁRIO
        @MasterNome,
        1, -- Ativo = true
        @MasterPasswordHash,
        @MasterPasswordHash,
        @MasterCpf,
        @CurrentDate,
        0, -- CreatedBy (Placeholder 0 para seed inicial)
        @CurrentDate,
        0, -- UpdatedBy (Placeholder 0 para seed inicial)
        0  -- IsDeleted = false
    );
    PRINT 'Usuário Master (Conta Corrente) inserido no ContaCorrenteDB.';
END
ELSE
BEGIN
    PRINT 'O usuário Master (Conta Corrente) já existe no ContaCorrenteDB. Pulando a inserção.';
END
GO

----------------------------------------------------------------------------------------------
---- BANCO CONTACORRENTE
USE [master]
GO
/****** Object:  Database [BankMoreTransferenciaDBB]    Script Date: 19/11/2025 01:22:46 ******/
CREATE DATABASE [BankMoreTransferenciaDB]
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BankMoreTransferenciaDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET ANSI_NULLS OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET ANSI_PADDING OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET ARITHABORT OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET  ENABLE_BROKER
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET ALLOW_SNAPSHOT_ISOLATION ON
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET READ_COMMITTED_SNAPSHOT ON
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET RECOVERY FULL
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET  MULTI_USER
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [BankMoreTransferenciaDB] SET DB_CHAINING OFF
GO

USE [BankMoreTransferenciaDB]
GO

CREATE TABLE transferencia (
    id UNIQUEIDENTIFIER PRIMARY KEY, -- identificacao unica da transferencia (GUID/UUID)
    idcontacorrenteorigem UNIQUEIDENTIFIER NOT NULL, -- identificacao unica da conta corrente de origem (GUID/UUID)
    idcontacorrentedestino UNIQUEIDENTIFIER NOT NULL, -- identificacao unica da conta corrente de destino (GUID/UUID)
    datamovimento DATETIME2 NOT NULL, -- data e hora exata da transferencia
    valor DECIMAL(10, 2) NOT NULL, -- valor da transferencia. Usar duas casas decimais.
	status INTEGER NOT NULL,
	dataultimaalteracao DATETIME2 NULL,
	erro VARCHAR(MAX),
	    [CreatedAt] [datetime2](7) NOT NULL,
    [CreatedBy] INT NOT NULL,
    [UpdatedAt] [datetime2](7) NOT NULL,
    [UpdatedBy] INT NOT NULL,
    [IsDeleted] BIT NOT NULL
);
GO

CREATE TABLE idempotencia (
    id UNIQUEIDENTIFIER PRIMARY KEY,        -- Chave de Idempotência (GUID/UUID)
    idcontacorrente UNIQUEIDENTIFIER NOT NULL, -- Chave para a conta que iniciou a requisição
    requisicao NVARCHAR(MAX),               -- Dados de Requisição (Body/Headers)
    resultado NVARCHAR(MAX),                -- Dados de Retorno (Resposta HTTP)
	    [CreatedAt] [datetime2](7) NOT NULL,
    [CreatedBy] INT NOT NULL,
    [UpdatedAt] [datetime2](7) NOT NULL,
    [UpdatedBy] INT NOT NULL,
    [IsDeleted] BIT NOT NULL,   
);

GO


CREATE TABLE outboxMessages (
    Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,     
    CreatedOn DATETIME2(7) NOT NULL,    
    Type NVARCHAR(255) NOT NULL,
    Payload NVARCHAR(MAX) NOT NULL,
    
    IsProcessed BIT NOT NULL DEFAULT 0,
  
    CorrelationId UNIQUEIDENTIFIER NULL 
);
GO

CREATE NONCLUSTERED INDEX IX_Outbox_Pending 
ON OutboxMessages (IsProcessed, CreatedOn)
WHERE IsProcessed = 0;
GO