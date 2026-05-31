namespace Rdm.Api.Inrastructure.Persistence;

using Microsoft.Extensions.Options;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Inrastructure.Configuration;
using Supabase;

public class DatabaseContext : IDatabaseContext<Client>
{
    private readonly Client _client;
    private readonly ILogger<DatabaseContext> _logger;

    public DatabaseContext(IOptions<SupabaseSettings> options, ILogger<DatabaseContext> logger)
    {
        this._logger = logger;
        var configuration = options.Value;
        if (string.IsNullOrWhiteSpace(configuration.Url))
        {
            this._logger.LogError("Error initialising database context: Url is empty");
            throw new InvalidOperationException("Failed to compose DatabaseClient connection Url is missing.");
        }

        if (string.IsNullOrWhiteSpace(configuration.ApiKey))
        {
            this._logger.LogError("Error initialising Database context: ApiKey is empty");
            throw new InvalidOperationException("Failed to compose DatabaseClient connection ApiKey is missing.");
        }

        var supabaseOptions = new SupabaseOptions
        {
            Schema = "result_data_manager",
        };

        this._client = new Client(configuration.Url, configuration.ApiKey, supabaseOptions);
    }

    public Client GetClient()
    {
        return this._client;
    }
}