using System.Net.Mail;

namespace BuildingBlocks.Common.Extensions;

public static class EmailAddressExtensions
{
    public static bool IsValidEmail(this string emailAddress)
    {
        try
        {
            MailAddress m = new(emailAddress);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
    
}