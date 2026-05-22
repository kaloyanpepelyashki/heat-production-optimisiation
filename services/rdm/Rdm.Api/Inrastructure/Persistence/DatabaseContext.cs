using Microsoft.Extensions.Options;
using Rdm.Api.Application.Interfaces;
using Rdm.Api.Inrastructure.Configuration;
using Supabase;

namespace Rdm.Api.Inrastructure.Persistence;


/// This class is soley responsible for exposing the database client (supabase client), to services that need it.
/// The class is closed for any modification.

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

    
    /// Used to get the database client. Returns an instance of the database client used for quering (or other operations) the database
    
    /// <returns>
    /// Client - supabase client instance
    /// </returns>
    public Client GetClient()
    {
        return _client;
    }
}