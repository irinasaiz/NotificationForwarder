using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NotificationForwarder.Tests;

[TestClass]
public class NotificationForwarderEndToEndTests
{
    private WebApplicationFactory<Program>? _factory;
    private HttpClient? _client;
    private const string OutputFilePath = "forwarded_notifications.txt";

    [TestInitialize]
    public void Setup()
    {
        // Clean up any existing output file before each test
        if (File.Exists(OutputFilePath))
        {
            File.Delete(OutputFilePath);
        }

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
            });
        
        _client = _factory.CreateClient();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
        
        // Clean up output file after each test
        if (File.Exists(OutputFilePath))
        {
            File.Delete(OutputFilePath);
        }
    }

    [TestMethod]
    public async Task PostNotification_WithWarningType_ShouldWriteToOutputFile()
    {
        // Arrange
        var warningNotification = """
            {
                "Type": "Warning",
                "Name": "Backup Failure",
                "Description": "The backup failed due to a database problem"
            }
            """;

        // Act
        var response = await PostNotification(warningNotification);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response should be OK");
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<bool>(responseContent);
        Assert.IsTrue(result, "API should return true for Warning notification");
        
        // Verify the notification was written to the output file
        Assert.IsTrue(File.Exists(OutputFilePath), "Output file should exist after Warning notification");
        
        var fileContent = await File.ReadAllTextAsync(OutputFilePath);
        Assert.IsTrue(fileContent.Contains("Backup Failure"), "File should contain notification name");
        Assert.IsTrue(fileContent.Contains("Warning"), "File should contain notification type");
        Assert.IsTrue(fileContent.Contains("database problem"), "File should contain notification description");
    }

    [TestMethod]
    public async Task PostNotification_WithInfoType_ShouldNotWriteToOutputFile()
    {
        // Arrange
        var infoNotification = """
            {
                "Type": "Info",
                "Name": "Quota Exceeded",
                "Description": "Compute Quota exceeded"
            }
            """;

        // Act
        var response = await PostNotification(infoNotification);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "Response should be OK");
        
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<bool>(responseContent);
        Assert.IsTrue(result, "API should return true for Info notification (even though ignored)");
        
        // Verify no file was created (Info notifications should be ignored)
        Assert.IsFalse(File.Exists(OutputFilePath), "Output file should NOT exist for Info notification");
    }

    [TestMethod]
    public async Task PostNotification_WithInvalidType_ShouldReturnBadRequest()
    {
        // Arrange
        var invalidNotification = """
            {
                "Type": "Info_wrong",
                "Name": "Quota Exceeded",
                "Description": "Compute Quota exceeded"
            }
            """;

        // Act
        var response = await PostNotification(invalidNotification);
        
        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode, "Response should be BadRequest");
        
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.IsTrue(responseContent.Contains("Invalid notification type"), 
            "Response should contain validation error message");
        
        // Verify no file was created
        Assert.IsFalse(File.Exists(OutputFilePath), "Output file should NOT exist for invalid notification");
    }

    private async Task<HttpResponseMessage> PostNotification(string jsonContent)
    {
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        return await _client!.PostAsync("/api/notificationforwarder", content);
    }
}
