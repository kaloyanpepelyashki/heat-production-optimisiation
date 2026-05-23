namespace Am.Api.Infrastructure.Presistence;

using Am.Api.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Supabase;

/// <summary>
/// This class is soley responsible for exposing the database client (supabase client), to services that need it.
/// The class is closed for any modification.
/// </summary>
public sealed class DatabaseContext
{
    private Client _supabaseClient;

    public DatabaseContext(IOptions<SupabaseSettings> options)
    {
            var configuration = options.Value;

            if (string.IsNullOrWhiteSpace(configuration.Url))
            {
                throw new InvalidOperationException("Failed to compose DatabaseClient connection Url is missing.");
            }

            if (string.IsNullOrWhiteSpace(configuration.ApiKey))
            {
                throw new InvalidOperationException("Failed to compose DatabaseClient connection ApiKey is missing.");
            }

            var SupabaseOptions = new SupabaseOptions
            {
                Schema = "production_units",
            };

            this._supabaseClient = new Client(configuration.Url, configuration.ApiKey, SupabaseOptions);
    }

    /// <summary>
    /// Used to get the superbase client. Returns an instance of the client client
    /// </summary>
    /// <returns>
    /// Client - supabase client instance
    /// </returns>
    public Client GetClient()
    {
        return this._supabaseClient;
    }
}