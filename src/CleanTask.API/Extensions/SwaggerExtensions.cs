using Microsoft.OpenApi.Models;

namespace CleanTask.API.Extensions;

public static class SwaggerExtensions
{
    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CleanTask API",
                Version = "v1",
                Description = """
                    A production-ready Task Management REST API built with:
                    - ASP.NET Core 8 — Clean Architecture
                    - CQRS with MediatR
                    - FluentValidation pipeline
                    - JWT Bearer authentication
                    - Entity Framework Core (SQL Server + PostgreSQL)
                    - Repository + Unit of Work pattern
                    - Soft delete
                    - Global exception handling middleware

                    Built by Anuradha Madhushani — Senior .NET Core & Azure Engineer
                    """,
                Contact = new OpenApiContact
                {
                    Name = "Anuradha Madhushani",
                    Email = "your@email.com"
                }
            });

            // Add JWT auth to Swagger UI
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token. Example: Bearer {your_token}"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
