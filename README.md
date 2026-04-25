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

Etapas 1 e 2 concluidas:

- solution criada
- projetos separados por camada
- referencias entre projetos configuradas
- bootstrap inicial da API preparado para evolucao
- entidades e enums do dominio modelados
- contratos base da camada Application definidos
- DTOs iniciais, filtros e modelos de paginacao adicionados

## Como executar

```bash
dotnet restore
dotnet build
dotnet run --project TaskManager.API
```

## Proximas etapas

- configuracao de persistencia com EF Core
- implementacao dos endpoints obrigatorios
- validacoes, servicos e regras de negocio
