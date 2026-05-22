using Am.Api.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Supabase;

namespace Am.Api.Infrastructure.Presistence;


public sealed class DatabaseContext
{
    private Client _supabaseClient; 
   
    public DatabaseContext(IOptions<SupabaseSettings> options)
    {
            var configuration = options.Value;
            
            if (string.IsNullOrWhiteSpace(configuration.Url))
                throw new InvalidOperationException("Failed to compose DatabaseClient connection Url is missing.");

            if (string.IsNullOrWhiteSpace(configuration.ApiKey))
                throw new InvalidOperationException("Failed to compose DatabaseClient connection ApiKey is missing.");


            var SupabaseOptions = new SupabaseOptions
            {
                Schema = "production_units"
            }; 
            
            _supabaseClient = new Client(configuration.Url, configuration.ApiKey, SupabaseOptions);
            
    }
    
    public Client GetClient()
    {
        return _supabaseClient;
    }
}