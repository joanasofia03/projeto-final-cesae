# projeto-final-cesae

Este projeto faz parte do plano final integrado da **Academia Natixis 2025** e tem como objetivo aplicar na prática os conhecimentos adquiridos em desenvolvimento backend, frontend, base de dados, segurança, visualização de dados e metodologias ágeis.

Atualmente, **o projeto encontra-se concluído.**

# 📌 Tema
**Plataforma Web Bancária com Integração de Cotações Reais de Ativos Financeiros**

A aplicação tem como objetivo oferecer ao utilizador uma visão centralizada das suas finanças pessoais e investimentos, com dados em tempo real de ações e criptomoedas, promovendo uma experiência moderna e informativa no contexto bancário.

# ⚙️ Tecnologias

| Camada        | Tecnologia            |
| ------------- | --------------------- |
| Backend       | ASP.NET Core Web API  |
| Base de Dados | SQL Server            |
| Frontend      | Angular               |
| Visualização  | Power BI              |
| Segurança     | JWT, Hashing, Logs    |
| API Externa   | CryptoCompare         |
| Gestão Ágil   | Trello                |

# 🧱 Estrutura Atual do Backend 

/Crysta

├── Controllers/

├── Dbcontext/

├── DTOs/

├── Helpers/

├── Migrations/

├── Models/

├── Services/

├── appsettings.json

├── JwtSettings.cs

├── TransactionSettings.cs

└── Program.cs

# ▶️ Como Executar o Projeto (Backend)

- **Clonar o repositório:**
  ```bash
    git clone https://github.com/joanaperpetuo263162630/projeto-final-cesae.git

- **Aceder à pasta do backend:**
  ```bash
    cd Crysta

- **Configurar a base de dados no appsettings.json**

- **Correr as migrations:**
  ```bash
    dotnet ef database update

- **Iniciar a aplicação:**
  ```bash
    dotnet run

# ▶️ Como Executar o Projeto (Frontend)

## 📦 Pré-requisitos

- **Node.js** (versão LTS recomendada)
- **Angular CLI** instalado globalmente:
  
  ```bash
  npm install -g @angular/cli

- **Aceder à pasta do frontend:**

    ```bash
    cd projeto-final-cesae/Crysta-app

- **Instalar as dependências:**

    ```bash
    npm install

- **Iniciar a aplicação:**
    ```bash
    ng serve

- **Abrir no navegador:**

A aplicação estará disponível em:
http://localhost:4200

# 📊 Power BI
A exportação de dados será disponibilizada para visualização via Power BI, permitindo análises financeiras e insights avançados com foco em storytelling e apoio à decisão. 
