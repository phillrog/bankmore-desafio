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
/****** Object:  Database [BankMoreContaCorretenDB]    Script Date: 15/11/2025 10:00:46 ******/
CREATE DATABASE [BankMoreContaCorretenDB]
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BankMoreContaCorretenDB].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET ANSI_NULLS OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET ANSI_PADDING OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET ARITHABORT OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET  ENABLE_BROKER
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET ALLOW_SNAPSHOT_ISOLATION ON
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET READ_COMMITTED_SNAPSHOT ON
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET RECOVERY FULL
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET  MULTI_USER
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [BankMoreContaCorretenDB] SET DB_CHAINING OFF
GO

USE [BankMoreContaCorretenDB]
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