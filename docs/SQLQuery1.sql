-- Criação da Base de Dados
CREATE DATABASE AnalyticPlatform;
GO

CREATE TABLE AppRole (
    Id INT PRIMARY KEY IDENTITY,
    RoleName NVARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE AppUser (
    Id INT PRIMARY KEY IDENTITY,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    PhoneNumber NVARCHAR(20) UNIQUE,
    DocumentId NVARCHAR(20) UNIQUE NOT NULL,
    DeletedAt DATETIME NULL,
    BirthDate DATE NOT NULL,
    CreationDate DATETIME DEFAULT GETDATE()
);

CREATE TABLE AppUserRole (
    AppUserId INT FOREIGN KEY REFERENCES AppUser(Id),
    AppRoleId INT FOREIGN KEY REFERENCES AppRole(Id),
    PRIMARY KEY (AppUserId, AppRoleId)
);

 
-- ========================
-- TABELAS DIMENSIONAIS
-- ========================
 
-- Dimensão: Tempo
CREATE TABLE DimTime (
    Id INT PRIMARY KEY,
    DimTimeDate DATE,
    DimTimeYear INT,
    DimTimeMonth INT,
    DimTimeQuarter INT,
    DimTimeDayOfWeek VARCHAR(10)
);

-- Dimensão: Conta
CREATE TABLE DimAccount (
    Id INT PRIMARY KEY,
    AccountNumber VARCHAR(20),
    AppUserId INT references AppUser(Id),
    FullName NVARCHAR(100),
    Region VARCHAR(50),
    Active BIT
);

-- Dimensão: Tipo de Transação
CREATE TABLE DimTransactionType (
    Id INT PRIMARY KEY,
    DimTransactionType_Type VARCHAR(20) -- Ex: 'transferencia', 'pagamento'
);

-- Dimensão: Ativo Financeiro
CREATE TABLE DimAsset (
    Id INT PRIMARY KEY,
    Symbol VARCHAR(10),
    DimAssetName VARCHAR(50),
    DimAssetType VARCHAR(20)
);
 
-- ========================
-- TABELAS DE FATO
-- ========================

-- Fato: Transações Bancárias
CREATE TABLE FactTransaction (
    Id INT PRIMARY KEY IDENTITY,
    DateId INT FOREIGN KEY REFERENCES DimTime(Id),
    AccountOriginId INT FOREIGN KEY REFERENCES DimAccount(Id),
    AccountDestinationId INT FOREIGN KEY REFERENCES DimAccount(Id),
    TypeTransactionId INT FOREIGN KEY REFERENCES DimTransactionType(Id),
    ValueTransaction DECIMAL(18, 2) NOT NULL
);

-- Fato: Cotações de Ativos
CREATE TABLE FactCotation (
    Id INT PRIMARY KEY IDENTITY,
    DimTimeId INT FOREIGN KEY REFERENCES DimTime(Id),
    DimAssetId INT FOREIGN KEY REFERENCES DimAsset(Id),
    Price DECIMAL(18, 6)
);
