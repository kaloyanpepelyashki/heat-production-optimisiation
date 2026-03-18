namespace Am.Api.Application.Exceptions;

public class NoAssetsFoundException: Exception
{
    public NoAssetsFoundException(string message) : base(message) {}
}