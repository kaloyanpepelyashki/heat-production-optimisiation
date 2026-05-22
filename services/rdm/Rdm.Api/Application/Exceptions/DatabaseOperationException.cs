namespace Rdm.Api.Application.Exceptions;



/// Represents a failed database operation. For example, when an inser operation fails

public class DatabaseOperationException: Exception
{
    public DatabaseOperationException(string message) : base(message) {}
    
    public DatabaseOperationException(string message, Exception innerException) : base(message, innerException) {}
}