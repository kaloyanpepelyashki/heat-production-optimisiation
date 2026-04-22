using Rdm.Api.Application.Interfaces;
using Rdm.Api.Inrastructure.Configuration;
using Rdm.Api.Inrastructure.Persistence;
using Scalar.AspNetCore;
using Supabase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.Configure<SupabaseSettings>(builder.Configuration.GetSection("SupabaseSettings"));
builder.Services.AddSingleton<IDatabaseContext<Client>, DatabaseContext>();
builder.Services.AddScoped<IResultRepository, ResultRepository>(); 

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
