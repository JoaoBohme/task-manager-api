using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TaskManager.Application.DTOs.Tasks;
using TaskManager.Application.DTOs.Users;

namespace TaskManager.API.Swagger;

public class SwaggerSchemaExampleFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(CreateUserDto))
        {
            schema.Example = new OpenApiObject
            {
                ["name"] = new OpenApiString("Joao Silva"),
                ["email"] = new OpenApiString("joao@taskmanager.com"),
                ["password"] = new OpenApiString("SenhaForte123")
            };
        }

        if (context.Type == typeof(UpdateUserDto))
        {
            schema.Example = new OpenApiObject
            {
                ["name"] = new OpenApiString("Joao Silva Atualizado"),
                ["email"] = new OpenApiString("joao.atualizado@taskmanager.com")
            };
        }

        if (context.Type == typeof(CreateTaskDto))
        {
            schema.Example = new OpenApiObject
            {
                ["title"] = new OpenApiString("Preparar entrega do teste"),
                ["description"] = new OpenApiString("Finalizar os endpoints obrigatorios e revisar o README."),
                ["priority"] = new OpenApiString("High"),
                ["userId"] = new OpenApiString("11111111-1111-1111-1111-111111111111")
            };
        }

        if (context.Type == typeof(UpdateTaskDto))
        {
            schema.Example = new OpenApiObject
            {
                ["title"] = new OpenApiString("Preparar entrega final"),
                ["description"] = new OpenApiString("Executar validacoes finais e ajustar documentacao."),
                ["status"] = new OpenApiString("InProgress"),
                ["priority"] = new OpenApiString("High")
            };
        }
    }
}
