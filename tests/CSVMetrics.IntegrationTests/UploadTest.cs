using System.Net.Http.Headers;
using CSVMetrics.API;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CSVMetrics.IntegrationTests;

public class CsvUploadIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public CsvUploadIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Upload_ValidCsv()
    {
        var csvContent = "Date;ExecutionTime;Value\n" +
                         "2024-01-15T10:30:00.0000Z;1.5;42.3\n" +
                         "2024-01-15T11:00:00.0000Z;2.0;38.7\n" +
                         "2024-01-15T12:15:00.0000Z;1.2;45.1\n";

        var fileName = $"test-{Guid.NewGuid()}.csv";

        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes(csvContent));
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
        content.Add(fileContent, "file", fileName);
        
        var response = await _client.PostAsync("/api/measurements/upload", content);
        
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"medianValue\":42.3", responseBody);
        Assert.Contains("\"maxValue\":45.1", responseBody);
        Assert.Contains("\"minValue\":38.7", responseBody);
    }
}