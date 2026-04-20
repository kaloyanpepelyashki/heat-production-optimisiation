namespace Dv.App.Services;

using System.Threading.Tasks;

public interface IApiService
{
    Task<T?> GetAsync<T>(BackendService service, string endpoint);

    Task<TResponse?> PostAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data);

    Task<TResponse?> PutAsync<TRequest, TResponse>(BackendService service, string endpoint, TRequest data);

    Task<bool> DeleteAsync(BackendService service, string endpoint);
}
