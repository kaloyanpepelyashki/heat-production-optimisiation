using Am.Api.Application.Interfaces;
using Am.Api.Application.Services;
using Am.Api.Infrastructure.Configuration;
using Am.Api.Infrastructure.Presistence;
using Am.Api.Domain.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<SupabaseSettings>(builder.Configuration.GetSection("SupabaseSettings"));

builder.Services.AddSingleton<DatabaseContext>();

builder.Services.AddScoped<IProductionUnitRepository<GasBoiler>, GasBoilerRepository>();
builder.Services.AddScoped<IProductionUnitRepository<OilBoiler>, OilBoilerRepository>();
builder.Services.AddScoped<IProductionUnitRepository<ElectricBoiler>, ElectricBoilerRepository>();
builder.Services.AddScoped<IProductionUnitRepository<GasMotor>, GasMotorRepository>();
builder.Services.AddScoped<IMaintenanceRepository, MaintenanceRepository>();
builder.Services.AddScoped<IProductionUnitService, ProductionUnitService>();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
