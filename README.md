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

Etapas 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 e 12 concluidas:

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
- autenticacao JWT e endpoint de login implementados
- todos os endpoints da API protegidos com JWT, exceto o login
- Docker Compose configurado para API, banco SQL Server e migration
- seed automatico e idempotente de usuario admin para facilitar avaliacao
- logs estruturados com Serilog configurados para a API
- cache em memoria nas listagens de tarefas e usuarios com invalidacao em escrita

## Como executar

Ambiente local:

```bash
dotnet restore
dotnet build
dotnet tool restore
dotnet ef database update --project TaskManager.Infrastructure --startup-project TaskManager.API
dotnet run --project TaskManager.API
```

Com Docker Compose:

```bash
docker compose up --build
```

Swagger:

```text
http://localhost:5064/swagger
```

Com Docker:

```text
http://localhost:8080/swagger
```

Login:

```text
POST /api/auth/login
```

Usuario admin de seed:

```text
email: admin@taskmanager.com
senha: Admin@TaskManager2026
```

## Proximas etapas

- refinamentos finais de banco e empacotamento
- README final de entrega
