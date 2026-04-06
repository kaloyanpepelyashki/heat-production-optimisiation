using Microsoft.Extensions.Options;
using Opt.Api.Application.Exceptions;
using Opt.Api.Infrastructure.Configuration;

namespace Opt.Api.Infrastructure.Persistence;

using Supabase;

public class DatabaseContext
{
    private readonly Client _supabseClient;

    public DatabaseContext(IOptions<SupabaseSettings> options)
    {
            var configuration = options.Value;
            
            if (string.IsNullOrWhiteSpace(configuration.Url))
                throw new InvalidOperationException("Failed to compose DatabaseClient connection Url is missing.");

            if (string.IsNullOrWhiteSpace(configuration.ApiKey))
                throw new InvalidOperationException("Failed to compose DatabaseClient connection ApiKey is missing.");

            var SupabaseOptions = new SupabaseOptions
            {
                Schema = "optimizer"
            };
            
            _supabseClient = new Client(configuration.Url, configuration.ApiKey, SupabaseOptions);
        
    }


    public Client GetClient()
    {
        return _supabseClient;
    }
}