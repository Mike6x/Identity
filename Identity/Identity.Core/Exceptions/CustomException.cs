namespace Identity.Core.Exceptions;

public class CustomException : BaseException
{
    public CustomException(string message, string errorCode, object[] parameters )
        : base(message, errorCode, parameters)
    {
    }
}
