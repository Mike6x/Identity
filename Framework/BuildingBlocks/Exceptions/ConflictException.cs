using System.Net;

namespace BuildingBlocks.Exceptions
{
    public class ConflictException : CustomException
{
    public ConflictException(string message)
        : base(message, null, HttpStatusCode.Conflict)
    {
    }
}
}
