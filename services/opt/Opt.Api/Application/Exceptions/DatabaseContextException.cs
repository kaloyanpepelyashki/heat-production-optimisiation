namespace Opt.Api.Application.Exceptions;

[Serializable]
public class DatabaseContextException : Exception
{
    public DatabaseContextException(string message) : base(message) {}
    public DatabaseContextException(string message, Exception innerException) : base(message, innerException) {}
}