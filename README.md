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

Etapas 1, 2, 3, 4, 5, 6, 7 e 8 concluidas:

- solution criada
- projetos separados por camada
- referencias entre projetos configuradas
- bootstrap inicial da API preparado para evolucao
- entidades e enums do dominio modelados
- contratos base da camada Application definidos
- DTOs iniciais, filtros e modelos de paginacao adicionados
- DbContext, configuracoes EF Core e repositorios implementados
- infraestrutura registrada via Dependency Injection
- base pronta para migrations e camada de servicos
- servicos de usuarios e tarefas implementados
- validacoes com FluentValidation adicionadas
- regra de hash de senha e tratamento de conflitos/not found preparados
- controllers base de usuarios e tarefas implementados
- API pronta para expor o CRUD principal
- listagem de tarefas com filtros e paginacao exposta via API
- endpoint de resumo por status do usuario adicionado
- middleware global de erros com resposta padronizada implementado
- Swagger/OpenAPI configurado com exemplos basicos para os DTOs principais
- testes unitarios adicionados para a camada de servicos

## Como executar

```bash
dotnet restore
dotnet build
dotnet run --project TaskManager.API
```

Swagger:

```text
http://localhost:5064/swagger
```

## Proximas etapas

- migrations e ajuste final da execucao local
- autenticacao e demais diferenciais
- refinamentos de banco, logs e empacotamento
