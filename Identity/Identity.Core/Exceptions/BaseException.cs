namespace Identity.Core.Exceptions;

public abstract class BaseException : Exception
{
    public string ErrorCode { get; }
    public object[] Parameters { get; }

    protected BaseException(string message, string errorCode = null, object[] parameters = null)
        : base(message)
    {
        ErrorCode = errorCode;
        Parameters = parameters;
    }
}
