namespace Identity.Core.Constants;

public static class ErrorCodes
{
    public const string InvalidCredentials = "AUTH001";
    public const string UserNotFound = "AUTH002";
    public const string UserLocked = "AUTH003";
    public const string InvalidToken = "AUTH004";
    public const string ExpiredToken = "AUTH005";
    public const string EmailAlreadyExists = "REG001";
    public const string UsernameAlreadyExists = "REG002";
}