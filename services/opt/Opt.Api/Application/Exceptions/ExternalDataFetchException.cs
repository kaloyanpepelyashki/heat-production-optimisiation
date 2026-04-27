namespace Opt.Api.Application.Exceptions;

public sealed class ExternalDataFetchException : Exception
{
    public ExternalDataFetchException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}