using Rdm.Api.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Supabase;

namespace Rdm.Api.Infrastructure.Persistence;

/// <summary>
/// This class is solely responsible for exposing the database client (supabase client) to services that need it.
/// The class is closed for any modification.
/// </summary>
public sealed class DatabaseContext
{
    private readonly Client _supabaseClient; 
   
    public DatabaseContext(IOptions<SupabaseSettings> options)
    {
        var configuration = options.Value;
            
        if (string.IsNullOrWhiteSpace(configuration.Url))
            throw new InvalidOperationException("Failed to compose DatabaseClient connection: Url is missing.");

        if (string.IsNullOrWhiteSpace(configuration.ApiKey))
            throw new InvalidOperationException("Failed to compose DatabaseClient connection: ApiKey is missing.");

        var supabaseOptions = new SupabaseOptions
        {
            Schema = "optimization_results" // Assume a schema, can be adjusted later
        }; 
            
        _supabaseClient = new Client(configuration.Url, configuration.ApiKey, supabaseOptions);
    }
    
    /// <summary>
    /// Used to get the Supabase client. Returns an instance of the client.
    /// </summary>
    /// <returns>
    /// Client - Supabase client instance.
    /// </returns>
    public Client GetClient()
    {
        return _supabaseClient;
    }
}