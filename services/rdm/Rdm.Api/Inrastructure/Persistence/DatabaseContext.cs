using Microsoft.Extensions.Options;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Inrastructure.Configuration;
using Supabase;

namespace Rdm.Api.Inrastructure.Persistence;


public class DatabaseContext : IDatabaseContext<Client>
{
    private readonly Client _client;
    private readonly ILogger<DatabaseContext> _logger;

    public DatabaseContext(IOptions<SupabaseSettings> options, ILogger<DatabaseContext> logger)
    {   
        _logger = logger;
        var configuration = options.Value;
        if (string.IsNullOrWhiteSpace(configuration.Url))
        {
            _logger.LogError("Error initialising database context: Url is empty");
            throw new InvalidOperationException("Failed to compose DatabaseClient connection Url is missing.");
        }

        if (string.IsNullOrWhiteSpace(configuration.ApiKey))
        {
            _logger.LogError("Error initialising Database context: ApiKey is empty");
            throw new InvalidOperationException("Failed to compose DatabaseClient connection ApiKey is missing.");
        }

        var supabaseOptions = new SupabaseOptions
        {
            Schema = "result_data_manager"
        };
        
        _client = new Client(configuration.Url, configuration.ApiKey, supabaseOptions);
    }

    public Client GetClient()
    {
        return _client;
    }
}