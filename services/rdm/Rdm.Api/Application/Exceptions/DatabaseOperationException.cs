namespace Rdm.Api.Application.Exceptions;


/// <summary>
/// Represents a failed database operation. For example, when an inser operation fails
/// </summary>
public class DatabaseOperationException: Exception
{
    public DatabaseOperationException(string message) : base(message) {}
    
    public DatabaseOperationException(string message, Exception innerException) : base(message, innerException) {}
}