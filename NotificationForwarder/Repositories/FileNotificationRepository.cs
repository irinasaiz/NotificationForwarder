using System.Text.Json;
using NotificationForwarder.DTOs;

namespace NotificationForwarder.Repositories;

public class FileNotificationRepository : INotificationRepository
{
    private readonly string _filePath;
    private readonly ILogger<FileNotificationRepository> _logger;

    public FileNotificationRepository(IConfiguration configuration, ILogger<FileNotificationRepository> logger)
    {
        _filePath = configuration.GetValue<string>("NotificationSettings:FilePath") ?? "notifications.txt";
        _logger = logger;
    }

    public async Task WriteNotificationAsync(string content, CancellationToken cancellationToken = default)
    {
        try
        {
            var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC");
            var logEntry = $"[{timestamp}] {content}{Environment.NewLine}";
            
            await File.AppendAllTextAsync(_filePath, logEntry, cancellationToken);
            _logger.LogInformation("Notification written to file: {FilePath}", _filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write notification to file: {FilePath}", _filePath);
            throw;
        }
    }
}