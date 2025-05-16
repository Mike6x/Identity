namespace Identity.Core.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message)
        : base(message, "NotFound")
    {
    }
}
