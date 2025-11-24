-- Este script contém apenas comandos DDL/DML, sem CREATE DATABASE, USE ou ALTER DATABASE,
-- pois o banco de dados 'db-identidade' já foi criado pelo Terraform,
-- e o contexto de conexão é definido pelo sqlcmd (parâmetro -d).

PRINT 'Iniciando criação de esquema e seed de dados para Identity DB...';
GO

-- 1. Criação das Tabelas do ASP.NET Identity e Customers

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- AspNetRoleClaims
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoleClaims]') AND type in (N'U'))
BEGIN
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
END
GO

-- AspNetRoles
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetRoles]') AND type in (N'U'))
BEGIN
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
END
GO

-- AspNetUserClaims
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserClaims]') AND type in (N'U'))
BEGIN
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
END
GO

-- AspNetUserLogins
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserLogins]') AND type in (N'U'))
BEGIN
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
END
GO

-- AspNetUserRoles
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserRoles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AspNetUserRoles](
        [UserId] [nvarchar](450) NOT NULL,
        [RoleId] [nvarchar](450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED 
    (
        [UserId] ASC,
        [RoleId] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
    )
END
GO

-- AspNetUsers
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') AND type in (N'U'))
BEGIN
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
END
GO

-- AspNetUserTokens
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUserTokens]') AND type in (N'U'))
BEGIN
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
END
GO

-- Customers
SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
BEGIN
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
END
GO

-- RefreshTokens
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RefreshTokens]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[RefreshTokens] (
      [Token] nvarchar(max) NOT NULL,
      [JwtId] nvarchar(max) NULL,
      [CreationDate] datetime2(0) NULL,
      [ExpiryDate] datetime2(0) NULL,
      [Used] bit NULL,
      [Invalidated] bit NULL,
      [UserId] nvarchar(450) NULL
    )
END
GO

-- 2. Inserções Iniciais (Seed de Roles e Usuário Master)

-- Inserções Iniciais (Seed de Roles)
IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE Name = 'Admin')
    INSERT INTO [dbo].[AspNetRoles]([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'4f7d42de-333e-43a7-afd8-09715f14a08c', N'e2ec2f11-acef-443c-9e38-2fd64ffd7b92', N'Admin', N'ADMIN');

IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE Name = 'Master')
    INSERT INTO [dbo].[AspNetRoles]([Id], [ConcurrencyStamp], [Name], [NormalizedName]) VALUES (N'14b9234c-e7e6-4cf7-8a7b-82467ecc6b6b', N'14b9234c-e7e6-4cf7-8a7b-82467ecc6b6b', N'Master', N'MASTER');
GO
SET ANSI_PADDING OFF
GO

-- Seed de Usuário Master
DECLARE @CurrentDate DATETIME2(7) = GETDATE();
-- NOTE: A senha é 'MinhaSenhaSegura123!' (HASH: 10000.YXwK4ASk1S6XvxAGMEDEIw==.IbEYeANMiR1CsNftByIP0flRlAtxGUP4uns7c6o6lCI=)
DECLARE @MasterPasswordHash NVARCHAR(MAX) = N'10000.YXwK4ASk1S6XvxAGMEDEIw==.IbEYeANMiR1CsNftByIP0flRlAtxGUP4uns7c6o6lCI=';
DECLARE @MasterId NVARCHAR(100) = N'C40CB0C3-D8D2-474D-B172-21785D5F71F8'; 
DECLARE @MasterRoleId NVARCHAR(450) = (SELECT ID FROM [dbo].[AspNetRoles] WHERE NormalizedName = 'MASTER');
DECLARE @MasterLogin NVARCHAR(20) = N'00000000000';
DECLARE @MasterNormalizedUserName NVARCHAR(256) = @MasterLogin;

PRINT 'Iniciando Seed de Usuário Master no Identity DB...';

IF EXISTS (SELECT 1 FROM [dbo].[AspNetRoles] WHERE NormalizedName = 'MASTER')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [dbo].[AspNetUsers] WHERE Id = @MasterId)
    BEGIN
        INSERT INTO [dbo].[AspNetUsers]
        (
            [Id], [AccessFailedCount], [ConcurrencyStamp], [EmailConfirmed], [LockoutEnabled], [LockoutEnd], 
            [PhoneNumberConfirmed], [TwoFactorEnabled], [UserName], [NormalizedUserName], [Cpf], [PasswordHash], [SecurityStamp]
        )
        VALUES
        (
            @MasterId, 0, NEWID(), 1, 1, NULL, 0, 0, @MasterLogin, @MasterNormalizedUserName, @MasterLogin, @MasterPasswordHash, NEWID()
        );

        INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId])
        VALUES (@MasterId, @MasterRoleId);
        
        PRINT 'Usuário Master inserido e Role Master atribuída.';
    END
    ELSE
    BEGIN
        PRINT 'O usuário Master já existe. Pulando a inserção.';
    END
END
ELSE
BEGIN
    PRINT 'ERRO: A Role Master não foi encontrada. O Seed do usuário Master foi pulado.';
END
GO