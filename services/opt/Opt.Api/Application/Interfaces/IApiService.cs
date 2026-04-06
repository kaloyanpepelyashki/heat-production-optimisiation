namespace Opt.Api.Application.Interfaces;

using System.Threading.Tasks;
using Opt.Api.Application.Services;

/// <summary>
/// Interface for the main data retrieval layer that interacts with the backend services.
/// </summary>
public interface IApiService
{
    Task<T?> GetAsync<T>(BackendService service, string endpoint);
}