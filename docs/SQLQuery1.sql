-- Criação da Base de Dados
CREATE DATABASE AnalyticPlatform;
GO

CREATE TABLE AppRole (
    id INT PRIMARY KEY IDENTITY,
    role_name NVARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE AppUser (
    id INT PRIMARY KEY IDENTITY,
    email NVARCHAR(100) UNIQUE NOT NULL,
    password_hash NVARCHAR(255) NOT NULL,
    fullname NVARCHAR(100),
    creation_date DATETIME DEFAULT GETDATE()
);

CREATE TABLE AppUserRole (
    appUser_id INT FOREIGN KEY REFERENCES AppUser(id),
    appRole_id INT FOREIGN KEY REFERENCES AppRole(id),
    PRIMARY KEY (appUser_id, appRole_id)
);

 
-- ========================
-- TABELAS DIMENSIONAIS
-- ========================
 
-- Dimensão: Tempo
CREATE TABLE DimTime (
    id INT PRIMARY KEY,
    dimtime_date DATE,
    dimtime_year INT,
    dimtime_month INT,
    dimtime_quarter INT,
    dimtime_day_of_week VARCHAR(10)
);

-- Dimensão: Conta
CREATE TABLE DimAccount (
    id INT PRIMARY KEY,
    account_number VARCHAR(20),
    appUser_id INT references AppUser(id),
    fullname NVARCHAR(100),
    region VARCHAR(50),
    active BIT
);

-- Dimensão: Tipo de Transação
CREATE TABLE DimTransactionType (
    id INT PRIMARY KEY,
    dimtransactiontype_type VARCHAR(20) -- Ex: 'transferencia', 'pagamento'
);

-- Dimensão: Ativo Financeiro
CREATE TABLE DimAsset (
    id INT PRIMARY KEY,
    symbol VARCHAR(10),
    dimasset_name VARCHAR(50),
    dimasset_type VARCHAR(20)
);
 
-- ========================
-- TABELAS DE FATO
-- ========================

-- Fato: Transações Bancárias
CREATE TABLE FactTransaction (
    id INT PRIMARY KEY IDENTITY,
    date_id INT FOREIGN KEY REFERENCES DimTime(id),
    account_origin_id INT FOREIGN KEY REFERENCES DimAccount(id),
    account_destination_id INT FOREIGN KEY REFERENCES DimAccount(id),
    type_transaction_id INT FOREIGN KEY REFERENCES DimTransactionType(id),
    value DECIMAL(18, 2) NOT NULL
);

-- Fato: Cotações de Ativos
CREATE TABLE FactCotation (
    id INT PRIMARY KEY IDENTITY,
    dimtime_id INT FOREIGN KEY REFERENCES DimTime(id),
    dimasset_id INT FOREIGN KEY REFERENCES DimAsset(id),
    price DECIMAL(18, 6)
);
