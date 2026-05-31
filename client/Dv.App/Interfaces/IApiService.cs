namespace Dv.App.Interfaces;

using System.Threading;
using System.Threading.Tasks;
using Dv.App.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(BackendService service, string endpoint);

    Task<TResponse?> PostAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data);

    Task<TResponse?> PutAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data);

    Task<bool> DeleteAsync(BackendService service, string endpoint);

    Task<bool> WakeUpService(BackendService service, CancellationToken token);

    Task<bool> WakeUpAllServices(CancellationToken token);
}
