# projeto-final-cesae

Este projeto faz parte do plano final integrado da **Academia Natixis 2025** e tem como objetivo aplicar na prática os conhecimentos adquiridos em desenvolvimento backend, frontend, base de dados, segurança, visualização de dados e metodologias ágeis.

Atualmente, **o foco está no desenvolvimento do backend com ASP.NET Web API e SQL Server.**

# 📌 Tema
**Plataforma Web Bancária com Integração de Cotações Reais de Ativos Financeiros**

A aplicação tem como objetivo oferecer ao utilizador uma visão centralizada das suas finanças pessoais e investimentos, com dados em tempo real de ações e criptomoedas, promovendo uma experiência moderna e informativa no contexto bancário.

# ⚙️ Tecnologias

| Camada        | Tecnologia            |
| ------------- | --------------------- |
| Backend       | ASP.NET Core Web API  |
| Base de Dados | SQL Server            |
| Frontend      | Angular *(futuro)*    |
| Visualização  | Power BI *(planeado)* |
| Segurança     | JWT, Hashing, Logs    |
| API Externa   | \[A definir]          |
| Gestão Ágil   | Trello                |

# 🧱 Estrutura Atual do Backend

/Crysta
├── Controllers/
├── Dbcontext/
├── DTOs/
├── Migrations/
├── Models/
├── appsettings.json
├── JwtSettings.cs
└── Program.cs

# ▶️ Como Executar o Projeto (Backend)

**Clonar o repositório:**

git clone https://github.com/joanaperpetuo263162630/projeto-final-cesae.git

**Aceder à pasta do backend:**

cd Crysta

**Configure a base de dados no appsettings.json**

**Correr as migrations:**

dotnet ef database update

**Iniciar a aplicação:**

dotnet run

# 📊 Power BI
A exportação de dados será disponibilizada para visualização via Power BI, permitindo análises financeiras e insights avançados com foco em storytelling e apoio à decisão.
