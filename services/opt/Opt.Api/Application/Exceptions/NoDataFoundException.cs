namespace Opt.Api.Application.Exceptions;

public class NoDataFoundException : Exception
{
    public NoDataFoundException(string message) : base(message) { }
    public NoDataFoundException(string message, Exception innerException) : base(message, innerException) { }
}