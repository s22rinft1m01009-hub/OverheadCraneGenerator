using CraneGenerator.Application.Drawing;
using CraneGenerator.Application.Drawing.Pdf;
using CraneGenerator.Application.Drawing.Pdf.Fonts;
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
using System.Text.Json;
using System.Text.Json.Serialization;

CraneFontResolver.Register();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

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
builder.Services.AddScoped<JibCraneGenerator>();
builder.Services.AddScoped<MonorailGenerator>();
builder.Services.AddScoped<WallMountedJibGenerator>();
builder.Services.AddScoped<PortableGantryGenerator>();
builder.Services.AddScoped<SemiGantryGenerator>();
builder.Services.AddScoped<UnderslungGenerator>();
builder.Services.AddScoped<CraneDrawingFactory>();
builder.Services.AddScoped<SvgDrawingRenderer>();
builder.Services.AddScoped<IPdfDrawingRenderer, PdfDrawingRenderer>();

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CraneDbContext>();
    db.Database.EnsureCreated();
}

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

public partial class Program { }
