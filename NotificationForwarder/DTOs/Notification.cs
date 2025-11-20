namespace NotificationForwarder.DTOs;
public class Notification
{
    public required string Type { get; set; }
    public required string Name { get; set; } = string.Empty;
    public required string Description { get; set; } = string.Empty;
}
