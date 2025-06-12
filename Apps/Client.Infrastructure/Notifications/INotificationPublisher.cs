namespace Client.Infrastructure.Notifications;

public interface INotificationPublisher
{
    Task PublishAsync(INotificationMessage notification);
}