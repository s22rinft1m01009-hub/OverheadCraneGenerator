using CraneGenerator.Application.DTOs;
using CraneGenerator.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace CraneGenerator.Tests;

public sealed class ApiAndPersistenceIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiAndPersistenceIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task FullDesignLifecycleAndDrawingEndpointsWork()
    {
        // 1. CREATE design
        var createRequest = new CreateDesignRequest
        {
            DesignName = "Integration Test EOT Crane",
            CraneType = "DOUBLE_GIRDER_EOT",
            CapacityTonnes = 16,
            SpanMm = 24000,
            LiftHeightMm = 9000,
            RunwayWidthMm = 18000,
            HoistPositionMm = 12000,
            BuildingHeightMm = 10500
        };

        var postResponse = await _client.PostAsJsonAsync("/api/designs", createRequest);
        Assert.Equal(HttpStatusCode.OK, postResponse.StatusCode);

        var postResult = await postResponse.Content.ReadFromJsonAsync<JsonElement>();
        var designData = postResult.GetProperty("data");
        var designId = designData.GetProperty("id").GetInt32();
        Assert.True(designId > 0);

        // 2. GET by ID
        var getResponse = await _client.GetAsync($"/api/designs/{designId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var getResult = await getResponse.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Integration Test EOT Crane", getResult.GetProperty("data").GetProperty("designName").GetString());

        // 3. GET all
        var getAllResponse = await _client.GetAsync("/api/designs");
        Assert.Equal(HttpStatusCode.OK, getAllResponse.StatusCode);

        // 4. UPDATE design
        var updateRequest = new UpdateDesignRequest
        {
            DesignName = "Updated Integration Crane",
            CraneType = "DOUBLE_GIRDER_EOT",
            CapacityTonnes = 20,
            SpanMm = 26000,
            LiftHeightMm = 9500,
            RunwayWidthMm = 18000,
            HoistPositionMm = 13000,
            BuildingHeightMm = 11000
        };
        var putResponse = await _client.PutAsJsonAsync($"/api/designs/{designId}", updateRequest);
        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

        // Verify update persisted
        var verifyGet = await _client.GetAsync($"/api/designs/{designId}");
        var verifyResult = await verifyGet.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Updated Integration Crane", verifyResult.GetProperty("data").GetProperty("designName").GetString());

        // 5. GENERATE DrawingModel JSON (verify polymorphic geometry)
        var generateResponse = await _client.PostAsync($"/api/designs/{designId}/generate", null);
        Assert.Equal(HttpStatusCode.OK, generateResponse.StatusCode);

        var jsonString = await generateResponse.Content.ReadAsStringAsync();
        Assert.Contains("\"$type\":\"rectangle\"", jsonString);
        Assert.Contains("\"width\":", jsonString);
        Assert.Contains("\"height\":", jsonString);
        Assert.Contains("\"elements\":", jsonString);
        Assert.Contains("\"details\":", jsonString);
        Assert.Contains("\"technicalData\":", jsonString);

        // 6. SVG Endpoint
        var svgResponse = await _client.GetAsync($"/api/designs/{designId}/drawing/svg");
        Assert.Equal(HttpStatusCode.OK, svgResponse.StatusCode);
        Assert.Equal("image/svg+xml", svgResponse.Content.Headers.ContentType?.MediaType);
        var svgContent = await svgResponse.Content.ReadAsStringAsync();
        Assert.Contains("<svg", svgContent);
        Assert.Contains("MAIN ELEVATION", svgContent);
        Assert.Contains("DETAIL Z", svgContent);
        Assert.Contains("TECHNICAL SPECIFICATIONS", svgContent);

        // 7. PDF Endpoint
        var pdfResponse = await _client.GetAsync($"/api/designs/{designId}/drawing/pdf");
        Assert.Equal(HttpStatusCode.OK, pdfResponse.StatusCode);
        Assert.Equal("application/pdf", pdfResponse.Content.Headers.ContentType?.MediaType);
        var pdfBytes = await pdfResponse.Content.ReadAsByteArrayAsync();
        Assert.True(pdfBytes.Length > 1000);
        Assert.StartsWith("%PDF-", System.Text.Encoding.ASCII.GetString(pdfBytes, 0, 5));

        // 8. DELETE design
        var deleteResponse = await _client.DeleteAsync($"/api/designs/{designId}");
        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        // Verify 404 after deletion
        var afterDeleteResponse = await _client.GetAsync($"/api/designs/{designId}");
        Assert.Equal(HttpStatusCode.NotFound, afterDeleteResponse.StatusCode);
    }

    [Fact]
    public async Task InvalidDesignReturnsBadRequest()
    {
        var invalidRequest = new CreateDesignRequest
        {
            DesignName = "",
            CapacityTonnes = -10,
            SpanMm = 0
        };

        var response = await _client.PostAsJsonAsync("/api/designs", invalidRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task NonexistentDesignReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/designs/99999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
