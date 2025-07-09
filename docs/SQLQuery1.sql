-- Criação da Base de Dados
CREATE DATABASE AnalyticPlatform;
GO

USE AnalyticPlatform;
GO

-- Tabela: AppRole
CREATE TABLE AppRole (
    ID INT IDENTITY PRIMARY KEY,
    RoleName VARCHAR(50) UNIQUE NOT NULL
);

-- Tabela: AppUser
CREATE TABLE AppUser (
    ID INT IDENTITY PRIMARY KEY,
    Email VARCHAR(100) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(100) NULL,
    PhoneNumber VARCHAR(20) UNIQUE NULL,
    DocumentId VARCHAR(20) UNIQUE NOT NULL,
    DeletedAt DATETIME NULL,
    BirthDate DATE NOT NULL,
    Region VARCHAR(50) NOT NULL,
    CreationDate DATETIME DEFAULT GETDATE()
);

-- Tabela: AppUserRole
CREATE TABLE AppUserRole (
    AppUser_ID INT NOT NULL,
    AppRole_ID INT NOT NULL,
    PRIMARY KEY (AppUser_ID, AppRole_ID),
    FOREIGN KEY (AppUser_ID) REFERENCES AppUser(ID),
    FOREIGN KEY (AppRole_ID) REFERENCES AppRole(ID)
);

-- Dimensão: Time
CREATE TABLE Dim_Time (
    ID INT IDENTITY PRIMARY KEY,
    date_Date DATE NOT NULL,
    date_Year INT NULL,
    date_Month INT NULL,
    date_Quarter INT NULL,
    Weekday_Name VARCHAR(10) NULL,
    Is_Weekend BIT NOT NULL -- Tipo booleano em SQL Server é BIT
);

-- Dimensão: Account
CREATE TABLE Dim_Account (
    ID INT IDENTITY PRIMARY KEY,
    Account_Type VARCHAR(30) NULL,
    Account_Status VARCHAR(20) NULL,
    User_ID INT NULL,
    Opening_Date DATE NULL,
    Currency VARCHAR(10) NULL,
    FOREIGN KEY (User_ID) REFERENCES AppUser(ID)
);

-- Dimensão: Transaction Type
CREATE TABLE Dim_Transaction_Type (
    ID INT IDENTITY PRIMARY KEY,
    Dim_Transaction_Type_Description VARCHAR(50) NULL
);

-- Dimensão: Market Asset
CREATE TABLE Dim_Market_Asset (
    ID INT IDENTITY PRIMARY KEY,
    Asset_Name VARCHAR(100) NULL,
    Asset_Type VARCHAR(30) NULL, -- Cryptocurrency, Stock
    Symbol VARCHAR(10) NULL,
    Base_Currency VARCHAR(10) NULL,
    API_Source VARCHAR(100) NULL
);

-- Fato: Transactions
CREATE TABLE Fact_Transactions (
    ID INT IDENTITY PRIMARY KEY,
    Source_Account_ID INT NULL,
    Destination_Account_ID INT NULL,
    Time_ID INT NULL,
    Transaction_Type_ID INT NULL,
    AppUser_ID INT NULL,
    Transaction_Amount DECIMAL(18,2) NULL,
    Balance_After_Transaction DECIMAL(18,2) NULL,
    Execution_Channel VARCHAR(50) NULL,
    Transaction_Status VARCHAR(30) NULL,

    FOREIGN KEY (Source_Account_ID) REFERENCES Dim_Account(ID),
    FOREIGN KEY (Destination_Account_ID) REFERENCES Dim_Account(ID),
    FOREIGN KEY (Time_ID) REFERENCES Dim_Time(ID),
    FOREIGN KEY (Transaction_Type_ID) REFERENCES Dim_Transaction_Type(ID),
    FOREIGN KEY (AppUser_ID) REFERENCES AppUser(ID)
);

-- Fato: Market Asset Historical Prices
CREATE TABLE Fact_Market_Asset_History (
    ID INT IDENTITY PRIMARY KEY,
    Asset_ID INT NULL,
    Time_ID INT NULL,
    Open_Price DECIMAL(18,4) NULL,
    Close_Price DECIMAL(18,4) NULL,
    Trading_Volume DECIMAL(18,2) NULL,

    FOREIGN KEY (Asset_ID) REFERENCES Dim_Market_Asset(ID),
    FOREIGN KEY (Time_ID) REFERENCES Dim_Time(ID)
);

-- Fato: Notifications / Alerts
CREATE TABLE Fact_Notifications (
    ID INT IDENTITY PRIMARY KEY,
    AppUser_ID INT NULL,
    Time_ID INT NULL,
    Notification_Type VARCHAR(50) NULL, -- e.g., Balance Alert, Login Alert
    Channel VARCHAR(20) NULL, -- Email, App
    Status VARCHAR(20) NULL, -- Read, Unread

    FOREIGN KEY (AppUser_ID) REFERENCES AppUser(ID),
    FOREIGN KEY (Time_ID) REFERENCES Dim_Time(ID)
);
