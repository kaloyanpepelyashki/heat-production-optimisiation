using Opt.Api.Application.Interfaces;
using Opt.Api.Application.Services;
using Opt.Api.Infrastructure.Clients;
using Opt.Api.Infrastructure.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.Configure<ExternalApiOptions>(builder.Configuration.GetSection(ExternalApiOptions.SectionName));

builder.Services.AddHttpClient<IAssetDataProvider, AmDataProvider>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ExternalApiOptions>>().Value;
    client.BaseAddress = new Uri(options.Am.BaseUrl);
});

builder.Services.AddHttpClient<ISourceDataProvider, SdmDataProvider>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<ExternalApiOptions>>().Value;
    client.BaseAddress = new Uri(options.Sdm.BaseUrl);
});

builder.Services.AddScoped<Optimizer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
