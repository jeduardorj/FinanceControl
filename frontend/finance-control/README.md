# Finance Control

![Backend CI](https://github.com/jeduardorj/FinanceControl/actions/workflows/backend-ci.yml/badge.svg)
![Frontend CI](https://github.com/jeduardorj/FinanceControl/actions/workflows/frontend-ci.yml/badge.svg)

Sistema de Controle Financeiro Pessoal desenvolvido com .NET 9 e Angular.

## Tecnologias

### Backend
- .NET 9 / ASP.NET Core Web API
- Entity Framework Core + SQL Server
- JWT Authentication + Refresh Token
- Clean Architecture
- Repository Pattern + Unit of Work
- FluentValidation + AutoMapper
- xUnit + Moq + FluentAssertions

### Frontend
- Angular 17+ (Standalone Components)
- Angular Signals
- Reactive Forms
- Chart.js
- Guards e Interceptors

## Arquitetura
FinanceControl/ # 📦 Raiz do projeto
│
├── 📂 src/ # 🧩 Código-fonte principal
│ │
│ ├── 🏛️ FinanceControl.Domain/ # Entidades, interfaces e exceções
│ │
│ ├── ⚙️ FinanceControl.Application/ # Serviços, DTOs e validações
│ │
│ ├── 🗄️ FinanceControl.Infrastructure/ # EF Core, repositórios e JWT
│ │
│ └── 🌐 FinanceControl.API/ # Controllers e middlewares
│
├── 🧪 tests/ # ✅ Testes automatizados
│ │
│ └── 🔬 FinanceControl.Tests/ # Testes unitários
│
└── 🎨 frontend/ # 🖥️ Interface do usuário
│
└── 📱 finance-control/ # Aplicação Angular

## Como Executar

### Backend

```bash
# Restaurar dependências
dotnet restore

# Aplicar migrations
dotnet ef database update --project src/FinanceControl.Infrastructure --startup-project src/FinanceControl.API

# Executar
dotnet run --project src/FinanceControl.API
```

### Frontend

```bash
cd frontend/finance-control
npm install
ng serve
```

### Testes

```bash
dotnet test
```

## Endpoints

| Método | Rota | Descrição | Auth |
|--------|------|-----------|------|
| POST | /api/users/register | Cadastro | Não |
| POST | /api/auth/login | Login | Não |
| POST | /api/auth/refresh | Refresh Token | Não |
| POST | /api/auth/revoke | Logout | Sim |
| GET | /api/categories | Listar categorias | Sim |
| POST | /api/categories | Criar categoria | Sim |
| PUT | /api/categories/{id} | Atualizar categoria | Sim |
| DELETE | /api/categories/{id} | Excluir categoria | Sim |
| GET | /api/transactions/paged | Listar transações | Sim |
| POST | /api/transactions | Criar transação | Sim |
| PUT | /api/transactions/{id} | Atualizar transação | Sim |
| DELETE | /api/transactions/{id} | Excluir transação | Sim |
| GET | /api/dashboard | Dashboard financeiro | Sim |

