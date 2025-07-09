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

 -- Dimension: Time
CREATE TABLE Dim_Time (
    Time_ID SERIAL PRIMARY KEY,
    Date DATE NOT NULL,
    Year INT,
    Month INT,
    Quarter INT,
    Weekday_Name VARCHAR(10),
    Is_Weekend BOOLEAN
);

-- Dimension: Account
CREATE TABLE Dim_Account (
    Account_ID SERIAL PRIMARY KEY,
    Account_Type VARCHAR(30),
    Account_Status VARCHAR(20),
    User_ID INT,
    Opening_Date DATE,
    Currency VARCHAR(10),
    FOREIGN KEY (User_ID) REFERENCES AppUser(Id)

-- Dimension: Transaction Type
CREATE TABLE Dim_Transaction_Type (
    Transaction_Type_ID SERIAL PRIMARY KEY,
    Description VARCHAR(50)
);

-- Dimension: Market Asset
CREATE TABLE Dim_Market_Asset (
    Asset_ID SERIAL PRIMARY KEY,
    Asset_Name VARCHAR(100),
    Asset_Type VARCHAR(30), -- Cryptocurrency, Stock
    Symbol VARCHAR(10),
    Base_Currency VARCHAR(10),
    API_Source VARCHAR(100)
);

-- Fact: Transactions
CREATE TABLE Fact_Transactions (
    Transaction_ID SERIAL PRIMARY KEY,
    Source_Account_ID INT,
    Destination_Account_ID INT,
    Time_ID INT,
    Transaction_Type_ID INT,
    User_ID INT,
    Transaction_Amount DECIMAL(18,2),
    Balance_After_Transaction DECIMAL(18,2),
    Execution_Channel VARCHAR(50),
    Transaction_Status VARCHAR(30),

    FOREIGN KEY (Source_Account_ID) REFERENCES Dim_Account(Account_ID),
    FOREIGN KEY (Destination_Account_ID) REFERENCES Dim_Account(Account_ID),
    FOREIGN KEY (Time_ID) REFERENCES Dim_Time(Time_ID),
    FOREIGN KEY (Transaction_Type_ID) REFERENCES Dim_Transaction_Type(Transaction_Type_ID),
    FOREIGN KEY (User_ID) REFERENCES AppUser(Id)
);

-- Fact: Market Asset Historical Prices
CREATE TABLE Fact_Market_Asset_History (
    History_ID SERIAL PRIMARY KEY,
    Asset_ID INT,
    Time_ID INT,
    Open_Price DECIMAL(18,4),
    Close_Price DECIMAL(18,4),
    Trading_Volume DECIMAL(18,2),

    FOREIGN KEY (Asset_ID) REFERENCES Dim_Market_Asset(Asset_ID),
    FOREIGN KEY (Time_ID) REFERENCES Dim_Time(Time_ID)
);

-- Fact: Notifications / Alerts
CREATE TABLE Fact_Notifications (
    Notification_ID SERIAL PRIMARY KEY,
    User_ID INT,
    Time_ID INT,
    Notification_Type VARCHAR(50), -- e.g., Balance Alert, Login Alert
    Channel VARCHAR(20), -- Email, App
    Status VARCHAR(20), -- Read, Unread

    FOREIGN KEY (User_ID) REFERENCES AppUser(Id),
    FOREIGN KEY (Time_ID) REFERENCES Dim_Time(Time_ID)
);
