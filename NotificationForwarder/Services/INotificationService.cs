using NotificationForwarder.DTOs;

namespace NotificationForwarder.Services;

public interface INotificationService
{
    Task<bool> ProcessNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
}
