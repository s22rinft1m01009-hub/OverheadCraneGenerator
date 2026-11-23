using CraneGenerator.Application.Services;
using CraneGenerator.Infrastructure.Data;
using CraneGenerator.Infrastructure.Repositories;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using OverheadCraneGenerator.Middleware;
using OverheadCraneGenerator.Shared;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAutoMapper(cfg => { }, typeof(DesignMappingProfile));

// 1. Register OpenAPI generation with the custom transformer
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddDbContext<CraneDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDesignRepository, DesignRepository>();
builder.Services.AddScoped<IDesignService, DesignService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    // 2. Map the OpenAPI endpoint and Scalar UI
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        // Optional: pre-fill a dev token or set preferred scheme
        options.WithPreferredScheme("Bearer");
        // options.AddHttpAuthentication("Bearer", auth => auth.Token = "your-dev-token");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

// 3. Transformer to add the Bearer security scheme to the OpenAPI document
public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

        // 1. Add the security scheme (new API: use IOpenApiSecurityScheme dictionary)
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token"
        };

        // 2. Add global security requirement (new API: property name is Security, and use OpenApiSecuritySchemeReference)
        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });

        return Task.CompletedTask;
    }
}