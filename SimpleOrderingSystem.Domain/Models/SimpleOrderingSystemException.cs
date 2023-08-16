using SimpleOrderingSystem.Domain.Extensions;

namespace SimpleOrderingSystem.Domain.Models;

public class SimpleOrderingSystemException: Exception
{
    /// <summary>
    /// Gets the canonical error code that represents an application level error.
    /// </summary>
    public SimpleOrderingSystemErrorType ErrorCode { get; }
    public string? Details {get; }
    

    public SimpleOrderingSystemException(SimpleOrderingSystemErrorType errorCode) : this(errorCode, null) { }

    public SimpleOrderingSystemException(SimpleOrderingSystemErrorType errorCode, string? details) 
        : base(BuildMessage(errorCode, details))
    {
        ErrorCode = errorCode;
        Details = details;
    }

    private static string BuildMessage(SimpleOrderingSystemErrorType errorCode, string? details = null)
    {
        var errorDescription = errorCode.GetDescription();

        var baseMessage = errorDescription == null? errorCode.ToString(): $"{errorCode.ToString()}: {errorDescription}";

        if(details is null)
        {
            return baseMessage;
        }

        return $"{baseMessage} \r\n Details: \r\n {details}";
    }
}