namespace Am.Api.Application.Interfaces;

public interface IProductionUnitRepository<T>
{
    Task<List<T>> GetAllAsync();
    Task<T> GetByIdAsync(int id);
}