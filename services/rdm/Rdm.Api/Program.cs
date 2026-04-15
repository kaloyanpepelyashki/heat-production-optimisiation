using Rdm.Api.Infrastructure.Configuration;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<SupabaseSettings>(builder.Configuration.GetSection("SupabaseSettings"));

builder.Services.AddSingleton<Rdm.Api.Infrastructure.Persistence.DatabaseContext>();
builder.Services.AddScoped<Rdm.Api.Application.Interfaces.IOptimizationResultRepository<Rdm.Api.Domain.Models.OptimizationResult>, Rdm.Api.Infrastructure.Persistence.OptimizationResultRepository>();

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
