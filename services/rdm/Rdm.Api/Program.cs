using Rdm.Api.Application.Interfaces;
using Rdm.Api.Application.Services;
using Rdm.Api.Infrastructure.Persistence;
using Rdm.Api.Infrastructure.Persistence.Repositories;
using Rdm.Api.Inrastructure.Configuration;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// Register RDM services
builder.Services.Configure<SupabaseSettings>(builder.Configuration.GetSection("SupabaseSettings"));
builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddScoped<IOptimizationResultRepository, OptimizationResultRepository>(sp =>
{
    var context = sp.GetRequiredService<DatabaseContext>();
    return new OptimizationResultRepository(context.GetClient());
});
builder.Services.AddScoped<IOptimizationResultService, OptimizationResultService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

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
