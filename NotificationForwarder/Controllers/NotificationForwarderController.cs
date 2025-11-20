using Microsoft.AspNetCore.Mvc;
using NotificationForwarder.DTOs;
using NotificationForwarder.Services;

namespace NotificationForwarder.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationForwarderController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationForwarderController> _logger;

    public NotificationForwarderController(INotificationService notificationService, ILogger<NotificationForwarderController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<bool>> Receive([FromBody] Notification req, CancellationToken ctx)
    {
        try
        {
            var validationResult = ValidateRequest(req);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed: {Error}", validationResult.ErrorMessage);
                return BadRequest(validationResult.ErrorMessage);
            }

            _logger.LogInformation("Received notification: {Name} of type {Type}", req.Name, req.Type);
            
            var result = await _notificationService.ProcessNotificationAsync(req, ctx);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing notification");
            return StatusCode(500, "Internal server error occurred while processing notification");
        }
    }

    private ValidationResult ValidateRequest(Notification? req)
    {
        if (req == null)
        {
            return new ValidationResult(false, "Notification cannot be null");
        }

        if (string.IsNullOrWhiteSpace(req.Type))
        {
            return new ValidationResult(false, "Notification type is required");
        }

        if (string.IsNullOrWhiteSpace(req.Name))
        {
            return new ValidationResult(false, "Notification name is required");
        }

        if (string.IsNullOrWhiteSpace(req.Description))
        {
            return new ValidationResult(false, "Notification description is required");
        }

        var validTypes = new[] { NotificationType.Info, NotificationType.Warning, NotificationType.Error };
        if (!validTypes.Contains(req.Type, StringComparer.OrdinalIgnoreCase))
        {
            return new ValidationResult(false, $"Invalid notification type '{req.Type}'. Valid types are: {string.Join(", ", validTypes)}");
        }

        return new ValidationResult(true, null);
    }

    private record ValidationResult(bool IsValid, string? ErrorMessage);
}
