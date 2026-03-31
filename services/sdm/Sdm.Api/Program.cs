using Scalar.AspNetCore;
using Sdm.Api.Application.Interfaces;
using Sdm.Api.Application.Services;
using Sdm.Api.Infrastructure.Configuration;
using Sdm.Api.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<SupabaseSettings>(builder.Configuration.GetSection("SupabaseSettings"));

builder.Services.AddSingleton<DatabaseContext>();
builder.Services.AddScoped<ISourceDataRepository, SourceDataRepository>();
builder.Services.AddScoped<ISourceDataService , SourceDataService>();

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
