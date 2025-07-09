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
CREATE TABLE Dim_Tempo (
    ID_Tempo SERIAL PRIMARY KEY,
    Data DATE NOT NULL,
    Ano INT,
    Mes INT,
    Trimestre INT,
    Dia_Semana VARCHAR(10),
    Fim_de_Semana BOOLEAN
);

-- Dimensão: Utilizador
CREATE TABLE Dim_Utilizador (
    ID_Utilizador SERIAL PRIMARY KEY,
    Nome VARCHAR(100),
    Email VARCHAR(100),
    Tipo_Utilizador VARCHAR(20), -- Cliente, Administrador
    Regiao VARCHAR(50),
    Data_Registro DATE
);

-- Dimensão: Conta
CREATE TABLE Dim_Conta (
    ID_Conta SERIAL PRIMARY KEY,
    Tipo_Conta VARCHAR(30),
    Estado_Conta VARCHAR(20),
    ID_Utilizador INT,
    Data_Abertura DATE,
    Moeda VARCHAR(10),
    FOREIGN KEY (ID_Utilizador) REFERENCES Dim_Utilizador(ID_Utilizador)
);

-- Dimensão: Tipo de Transação
CREATE TABLE Dim_Tipo_Transacao (
    ID_Tipo_Transacao SERIAL PRIMARY KEY,
    Descricao VARCHAR(50)
);

-- Dimensão: Ativo de Mercado
CREATE TABLE Dim_Ativo_Mercado (
    ID_Ativo SERIAL PRIMARY KEY,
    Nome_Ativo VARCHAR(100),
    Tipo_Ativo VARCHAR(30), -- Criptomoeda, Ação
    Simbolo VARCHAR(10),
    Moeda_Base VARCHAR(10),
    Fonte_API VARCHAR(100)
);

-- ========================
-- TABELAS DIMENSIONAIS
-- ========================
-- Fato: Transações
CREATE TABLE Fato_Transacoes (
    ID_Transacao SERIAL PRIMARY KEY,
    ID_Conta_Origem INT,
    ID_Conta_Destino INT,
    ID_Tempo INT,
    ID_Tipo_Transacao INT,
    ID_Utilizador INT,
    Valor_Transacao DECIMAL(18,2),
    Saldo_Apos_Transacao DECIMAL(18,2),
    Meio_Execucao VARCHAR(50),
    Estado_Transacao VARCHAR(30),

    FOREIGN KEY (ID_Conta_Origem) REFERENCES Dim_Conta(ID_Conta),
    FOREIGN KEY (ID_Conta_Destino) REFERENCES Dim_Conta(ID_Conta),
    FOREIGN KEY (ID_Tempo) REFERENCES Dim_Tempo(ID_Tempo),
    FOREIGN KEY (ID_Tipo_Transacao) REFERENCES Dim_Tipo_Transacao(ID_Tipo_Transacao),
    FOREIGN KEY (ID_Utilizador) REFERENCES Dim_Utilizador(ID_Utilizador)
);

-- Fato: Histórico de Ativos Financeiros
CREATE TABLE Fato_Ativos_Historico (
    ID_Historico SERIAL PRIMARY KEY,
    ID_Ativo INT,
    ID_Tempo INT,
    Preco_Abertura DECIMAL(18,4),
    Preco_Fecho DECIMAL(18,4),
    Volume DECIMAL(18,2),

    FOREIGN KEY (ID_Ativo) REFERENCES Dim_Ativo_Mercado(ID_Ativo),
    FOREIGN KEY (ID_Tempo) REFERENCES Dim_Tempo(ID_Tempo)
);

-- Fato: Notificações/Alertas
CREATE TABLE Fato_Notificacoes (
    ID_Notificacao SERIAL PRIMARY KEY,
    ID_Utilizador INT,
    ID_Tempo INT,
    Tipo VARCHAR(50), -- Ex: Alerta de saldo, alerta de login
    Canal VARCHAR(20), -- Email, App
    Estado VARCHAR(20), -- Lida, Não lida

    FOREIGN KEY (ID_Utilizador) REFERENCES Dim_Utilizador(ID_Utilizador),
    FOREIGN KEY (ID_Tempo) REFERENCES Dim_Tempo(ID_Tempo)
);
