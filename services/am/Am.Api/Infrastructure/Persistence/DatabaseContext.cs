using Am.Api.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Supabase;

namespace Am.Api.Infrastructure.Presistence;

/// <summary>
/// This class is soley responsible for exposing the database client (supabase client), to services that need it.
/// The class is closed for any modification. 
/// </summary>
public sealed class DatabaseContext
{
    private Client _supabaseClient; 
   
    public DatabaseContext(IOptions<SupabaseSettings> options)
    {
        try
        {
            var configuration = options.Value;

            var SupabaseOptions = new SupabaseOptions
            {
                Schema = "production_units"
            }; 
            
            _supabaseClient = new Client(configuration.Url, configuration.ApiKey, SupabaseOptions);
           
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error in DatabaseConbtext . Error initialising Database Context:  {e.Message}, {e.GetType()}");
        }
    }
    
    /// <summary>
    /// Used to get the superbase client. Returns an instance of the client client
    /// </summary>
    /// <returns>
    /// Client - supabase client instance
    /// </returns>
    public Client GetClient()
    {
        return _supabaseClient;
    }
}