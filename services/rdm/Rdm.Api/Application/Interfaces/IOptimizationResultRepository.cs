namespace Rdm.Api.Application.Interfaces;

public interface IOptimizationResultRepository<T>
{
    Task<List<T>> GetAllAsync();
    Task<T> GetByIdAsync(Guid id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}