using Am.Api.Application.Interfaces;
using Am.Api.Application.Services;
using Am.Api.Infrastructure.Configuration;
using Am.Api.Infrastructure.Presistence;
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

//TODO, THE <T> OF THE IProductionUnitRepository has to be changed to a domain model, not persistence model. 
builder.Services.AddScoped<IProductionUnitRepository<GasBoilerPersistence>, GasBoilerRepository>();
builder.Services.AddScoped<IProductionUnitRepository<OilBoilerPersistence>, OilBoilerRepository>();
builder.Services.AddScoped<IProductionUnitRepository<ElectricBoilerPersistence>, ElectricBoilerRepository>();
builder.Services.AddScoped<IProductionUnitRepository<GasMotorPersistence>, GasMotorRepository>();
//

//Registers the ProductionUnitService as a scope service (important for the dependency injection container. 
builder.Services.AddScoped<IProductionUnitService, ProductionUnitService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
