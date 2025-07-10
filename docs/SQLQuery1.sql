-- Criação da Base de Dados
CREATE DATABASE AnalyticPlatform;
GO

USE AnalyticPlatform;
GO

-- Tabela: AppRole
CREATE TABLE AppRole (
    ID INT IDENTITY,
    RoleName VARCHAR(50) UNIQUE NOT NULL,
    CONSTRAINT PK_AppRole_ID PRIMARY KEY (ID)
);

-- Tabela: AppUser
CREATE TABLE AppUser (
    ID INT IDENTITY,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(100) NULL,
    PhoneNumber VARCHAR(20) UNIQUE NULL,
    DocumentId VARCHAR(20) UNIQUE NOT NULL,
    DeletedAt DATETIME NULL,
    BirthDate DATE NOT NULL,
    Region VARCHAR(50) NOT NULL,
    CreationDate DATETIME DEFAULT GETDATE(),
    CONSTRAINT PK_AppUser_ID PRIMARY KEY (ID)
);

-- Tabela: AppUserRole
CREATE TABLE AppUserRole (
    AppUser_ID INT NOT NULL,
    AppRole_ID INT NOT NULL,
    CONSTRAINT PK_AppUserRole PRIMARY KEY (AppUser_ID, AppRole_ID),
    CONSTRAINT FK_AppUserRole_AppUser FOREIGN KEY (AppUser_ID) REFERENCES AppUser(ID),
    CONSTRAINT FK_AppUserRole_AppRole FOREIGN KEY (AppRole_ID) REFERENCES AppRole(ID)
);

-- Dimensão: Time
CREATE TABLE Dim_Time (
    ID INT IDENTITY,
    date_Date DATE NOT NULL,
    date_Year INT NULL,
    date_Month INT NULL,
    date_Quarter INT NULL,
    Weekday_Name VARCHAR(10) NULL,
    Is_Weekend BIT NOT NULL,
    CONSTRAINT PK_Dim_Time_ID PRIMARY KEY (ID)
);

-- Dimensão: Account
CREATE TABLE Dim_Account (
    ID INT IDENTITY,
    Account_Type VARCHAR(30) NULL,
    Account_Status VARCHAR(20) NULL,
    AppUser_ID INT NULL,
    Opening_Date DATE NULL,
    Currency VARCHAR(10) NULL,
    CONSTRAINT PK_Dim_Account_ID PRIMARY KEY (ID),
    CONSTRAINT FK_Dim_Account_User FOREIGN KEY (AppUser_ID) REFERENCES AppUser(ID)
);

-- Dimensão: Transaction Type
CREATE TABLE Dim_Transaction_Type (
    ID INT IDENTITY,
    Dim_Transaction_Type_Description VARCHAR(50) NULL,
    CONSTRAINT PK_Dim_Transaction_Type_ID PRIMARY KEY (ID)
);

-- Dimensão: Market Asset
CREATE TABLE Dim_Market_Asset (
    ID INT IDENTITY,
    Asset_Name VARCHAR(100) NULL,
    Asset_Type VARCHAR(30) NULL,
    Symbol VARCHAR(10) NULL,
    Base_Currency VARCHAR(10) NULL,
    API_Source VARCHAR(100) NULL,
    CONSTRAINT PK_Dim_Market_Asset_ID PRIMARY KEY (ID)
);

-- Fato: Transactions
CREATE TABLE Fact_Transactions (
    ID INT IDENTITY,
    Source_Account_ID INT NULL,
    Destination_Account_ID INT NULL,
    Time_ID INT NULL,
    Transaction_Type_ID INT NULL,
    AppUser_ID INT NULL,
    Transaction_Amount DECIMAL(18,2) NULL,
    Balance_After_Transaction DECIMAL(18,2) NULL,
    Execution_Channel VARCHAR(50) NULL,
    Transaction_Status VARCHAR(30) NULL,
    CONSTRAINT PK_Fact_Transactions_ID PRIMARY KEY (ID),
    CONSTRAINT FK_Fact_Transactions_SourceAccount FOREIGN KEY (Source_Account_ID) REFERENCES Dim_Account(ID),
    CONSTRAINT FK_Fact_Transactions_DestinationAccount FOREIGN KEY (Destination_Account_ID) REFERENCES Dim_Account(ID),
    CONSTRAINT FK_Fact_Transactions_Time FOREIGN KEY (Time_ID) REFERENCES Dim_Time(ID),
    CONSTRAINT FK_Fact_Transactions_TransactionType FOREIGN KEY (Transaction_Type_ID) REFERENCES Dim_Transaction_Type(ID),
    CONSTRAINT FK_Fact_Transactions_User FOREIGN KEY (AppUser_ID) REFERENCES AppUser(ID)
);

-- Fato: Market Asset Historical Prices
CREATE TABLE Fact_Market_Asset_History (
    ID INT IDENTITY,
    Asset_ID INT NULL,
    Time_ID INT NULL,
    Open_Price DECIMAL(18,4) NULL,
    Close_Price DECIMAL(18,4) NULL,
    Trading_Volume DECIMAL(18,2) NULL,
    CONSTRAINT PK_Fact_Market_Asset_History_ID PRIMARY KEY (ID),
    CONSTRAINT FK_Fact_Market_Asset_History_Asset FOREIGN KEY (Asset_ID) REFERENCES Dim_Market_Asset(ID),
    CONSTRAINT FK_Fact_Market_Asset_History_Time FOREIGN KEY (Time_ID) REFERENCES Dim_Time(ID)
);

-- Fato: Notifications / Alerts
CREATE TABLE Fact_Notifications (
    ID INT IDENTITY,
    AppUser_ID INT NULL,
    Time_ID INT NULL,
    Notification_Type VARCHAR(50) NULL,
    Channel VARCHAR(20) NULL,
    Status VARCHAR(20) NULL,
    CONSTRAINT PK_Fact_Notifications_ID PRIMARY KEY (ID),
    CONSTRAINT FK_Fact_Notifications_User FOREIGN KEY (AppUser_ID) REFERENCES AppUser(ID),
    CONSTRAINT FK_Fact_Notifications_Time FOREIGN KEY (Time_ID) REFERENCES Dim_Time(ID)
);
