namespace Client.Infrastructure.Notifications;

public record ConnectionStateChanged(ConnectionState State, string? Message) : INotificationMessage;