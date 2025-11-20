namespace NotificationForwarder.Repositories;

public interface INotificationRepository
{
    Task WriteNotificationAsync(string content, CancellationToken cancellationToken = default);
}