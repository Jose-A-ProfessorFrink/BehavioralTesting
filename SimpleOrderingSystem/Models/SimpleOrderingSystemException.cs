namespace SimpleOrderingSystem.Models;

public class SimpleOrderingSystemException: Exception
{
    public SimpleOrderingSystemException(SimpleOrderingSystemErrorType errorType) : this(errorType.ToString()) { }

    public SimpleOrderingSystemException(string message) : base(message) { }

    public SimpleOrderingSystemException(string message, Exception innerException) : base(message,innerException) { }
}