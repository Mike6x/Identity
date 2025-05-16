namespace Identity.Core.Exceptions;

public class CustomException : BaseException
{
    public CustomException(string message, string errorCode = null, object[] parameters = null)
        : base(message, errorCode, parameters)
    {
    }
}
