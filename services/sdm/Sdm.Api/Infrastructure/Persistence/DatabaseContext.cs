using Microsoft.Extensions.Options;
using Sdm.Api.Application.Exceptions;
using Sdm.Api.Infrastructure.Configuration;
using Supabase;

namespace Sdm.Api.Infrastructure.Persistence;

public class DatabaseContext
{
    private readonly Client _supabseClient;

    public DatabaseContext(IOptions<SupabaseSettings> options)
    {
        try
        {
            var _options = options.Value;

            var SupabaseOptions = new SupabaseOptions
            {
                Schema = "source_data_manager"
            };
            
            _supabseClient = new Client(_options.Url, _options.ApiKey, SupabaseOptions);
        }
        catch (Exception e)
        {
            throw new DatabaseContextException($"Error in DatabaseContext. Error initialising DatabaseContext: {e.Message}, {e.GetType()}", e);
        }
    }


    public Client GetClient()
    {
        return _supabseClient;
    }
}