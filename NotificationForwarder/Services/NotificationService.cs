using System.Text.Json;
using NotificationForwarder.DTOs;
using NotificationForwarder.Repositories;

namespace NotificationForwarder.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationRepository notificationRepository, ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task<bool> ProcessNotificationAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing notification of type: {Type}", notification.Type);

        return notification.Type.ToLowerInvariant() switch
        {
            "warning" or "error" => await ForwardNotificationAsync(notification, cancellationToken),
            "info" => await IgnoreNotificationAsync(notification, cancellationToken),
            _ => await HandleUnknownTypeAsync(notification, cancellationToken)
        };
    }

    private async Task<bool> ForwardNotificationAsync(Notification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Forwarding notification: {Name}", notification.Name);
        
        var notificationJson = JsonSerializer.Serialize(notification, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        
        await _notificationRepository.WriteNotificationAsync(notificationJson, cancellationToken);
        return true;
    }

    private Task<bool> IgnoreNotificationAsync(Notification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Ignoring info notification: {Name}", notification.Name);
        return Task.FromResult(true);
    }

    private Task<bool> HandleUnknownTypeAsync(Notification notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning("Unknown notification type: {Type}. Notification: {Name}", notification.Type, notification.Name);
        return Task.FromResult(false);
    }
}