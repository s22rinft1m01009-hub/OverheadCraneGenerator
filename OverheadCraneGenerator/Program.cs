using CraneGenerator.Application.Drawing;
using CraneGenerator.Application.CraneModels;
using CraneGenerator.Application.Calculations;
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

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

builder.Services.AddDbContext<CraneDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));








// Add services to the container.
builder.Services.AddScoped<IDesignRepository, DesignRepository>();
builder.Services.AddScoped<IDesignService, DesignService>();
builder.Services.AddScoped<ICraneCalculationService, CraneCalculationService>();
builder.Services.AddScoped<ICraneDrawingService, CraneDrawingService>();
builder.Services.AddScoped<SingleGirderEotGenerator>();
builder.Services.AddScoped<DoubleGirderEotGenerator>();
builder.Services.AddScoped<GantryGenerator>();
builder.Services.AddScoped<CraneDrawingFactory>();
builder.Services.AddScoped<SvgDrawingRenderer>();









var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.WithPreferredScheme("Bearer");
        
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

       
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token"
        };

        
        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });

        return Task.CompletedTask;
    }
}