using Client.Infrastructure.Notifications;

namespace Identity.Admin.Preferences;
// public class AppTablePreference : INotificationMessage
public class AppTablePreference
{
    public bool IsDense { get; set; }
    public bool IsStriped { get; set; }
    public bool HasBorder { get; set; }
    public bool IsHoverable { get; set; }
}