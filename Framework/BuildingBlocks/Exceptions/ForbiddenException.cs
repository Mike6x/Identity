using System.Net;

namespace BuildingBlocks.Exceptions;
public class ForbiddenException : GeneralException
{
    public ForbiddenException()
        : base("unauthorized", [], HttpStatusCode.Forbidden)
    {
    }
    public ForbiddenException(string message)
       : base(message, [], HttpStatusCode.Forbidden)
    {
    }
}
