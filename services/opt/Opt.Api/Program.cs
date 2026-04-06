using Scalar.AspNetCore;
using Opt.Api.Infrastructure.Configuration;
using Opt.Api.Infrastructure.Persistence;
using Opt.Api.Application.Interfaces;
using Opt.Api.Application.Services;
using Opt.Api.Infrastructure.DTOs;
using Supabase.Gotrue;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<SupabaseSettings>(builder.Configuration.GetSection("SupabaseSettings"));

builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddScoped<INetProductionCostRepository, NetProductionCostRepository>();
builder.Services.AddScoped<INetProductionCostService, NetProductionCostService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
