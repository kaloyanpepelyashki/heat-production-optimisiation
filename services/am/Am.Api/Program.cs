using Am.Api.Application.Interfaces;
using Am.Api.Application.Services;
using Am.Api.Infrastructure.Configuration;
using Am.Api.Infrastructure.Presistence;
using Am.Api.Domain.Models;
using Am.Api.Model.DTOs;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<SupabaseSettings>(builder.Configuration.GetSection("SupabaseSettings"));

builder.Services.AddSingleton<DatabaseContext>();

builder.Services.AddScoped<IProductionUnitRepository<GasBoiler>, GasBoilerRepository>();
builder.Services.AddScoped<IProductionUnitRepository<OilBoiler>, OilBoilerRepository>();
builder.Services.AddScoped<IProductionUnitRepository<ElectricBoiler>, ElectricBoilerRepository>();
builder.Services.AddScoped<IProductionUnitRepository<GasMotor>, GasMotorRepository>();
//

//Registers the ProductionUnitService as a scope service (important for the dependency injection container. 
builder.Services.AddScoped<IProductionUnitService, ProductionUnitService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//Todo - To be removed later
app.MapGet("/diag/config", (IConfiguration config, IHostEnvironment env) => new
{
    Environment = env.EnvironmentName,
    Url = config["SupabaseSettings:Url"],
    ApiKeyPresent = !string.IsNullOrWhiteSpace(config["SupabaseSettings:ApiKey"])
});

app.MapGet("/diag/supabase", (IConfiguration config) =>
{
    return config.AsEnumerable()
        .Where(kv => kv.Key.Contains("Supabase", StringComparison.OrdinalIgnoreCase))
        .Select(kv => new
        {
            kv.Key,
            ValuePresent = !string.IsNullOrWhiteSpace(kv.Value)
        });
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
