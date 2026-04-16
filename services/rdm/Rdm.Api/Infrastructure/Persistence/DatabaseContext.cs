namespace Rdm.Api.Infrastructure.Persistence;

using Microsoft.Extensions.Options;
using Rdm.Api.Inrastructure.Configuration;
using Supabase;

public sealed class DatabaseContext
{
    private Client _supabaseClient;

    public DatabaseContext(IOptions<SupabaseSettings> options)
    {
        var configuration = options.Value;

        var supabaseOptions = new SupabaseOptions
        {
            Schema = "result_data_manager"
        };

        _supabaseClient = new Client(configuration.Url, configuration.ApiKey, supabaseOptions);
    }

    public Client GetClient()
    {
        return _supabaseClient;
    }
}