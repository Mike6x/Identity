namespace Identity.Core.Exceptions;

public class ValidationException : BaseException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message, IDictionary<string, string[]> errors = null)
        : base(message, "ValidationError")
    {
        Errors = errors;
    }
}
