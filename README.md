# TaskManager

API REST para gerenciamento de tarefas desenvolvida em .NET 8, estruturada em camadas para atender ao teste tecnico de perfil Pleno/Senior.

## Objetivo

Construir uma API com:

- CRUD de usuarios
- CRUD de tarefas
- filtro e paginacao
- resumo por status
- base preparada para autenticacao, testes e evolucoes incrementais

## Stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- xUnit

## Estrutura da solucao

```text
TaskManager/
|- TaskManager.API/            # Controllers, Program.cs e configuracoes da API
|- TaskManager.Application/    # Casos de uso, DTOs, interfaces e validacoes
|- TaskManager.Domain/         # Entidades e regras centrais do dominio
|- TaskManager.Infrastructure/ # Persistencia, DbContext e repositorios
|- TaskManager.Tests/          # Testes automatizados
|- TaskManager.sln
|- global.json
|- README.md
```

## Status atual

Etapa 1 concluida:

- solution criada
- projetos separados por camada
- referencias entre projetos configuradas
- bootstrap inicial da API preparado para evolucao

## Como executar

```bash
dotnet restore
dotnet build
dotnet run --project TaskManager.API
```

## Proximas etapas

- modelagem do dominio
- contratos da camada Application
- configuracao de persistencia com EF Core
- implementacao dos endpoints obrigatorios
