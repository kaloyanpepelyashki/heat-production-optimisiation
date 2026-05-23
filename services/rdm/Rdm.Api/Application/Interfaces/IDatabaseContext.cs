namespace Rdm.Api.Application.Interfaces;

public interface IDatabaseContext<T>
{
    T GetClient();
}