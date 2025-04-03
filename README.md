# MCPnet - A Network for Multimodal Capability Protocol (MCP) Servers

MCPnet is a .NET client library that allows AI applications to connect with MCP servers through a centralized network. This library provides a clean, typed interface for discovering and invoking MCP capabilities across the network.

## What is MCP?

MCP (Multimodal Capability Protocol) is an open standard for AI applications (called "MCP Clients") to connect with MCP Servers. These servers provide a set of "tools" that AI applications can access, greatly extending their capabilities.

## What is MCPnet?

MCPnet is a centralized network of MCP Servers that makes it frictionless to find and use MCP capabilities. The network includes:

* Discovery of MCP servers and capabilities
* Ratings and reviews of servers
* Semantic search for finding the right tools
* Creation of server "remixes" that combine tools from different servers
* And much more...

Think of it as the "Hugging Face of MCP" - a way to discover and connect to MCP Servers.

## Installation

You can install the MCPnet package via NuGet:

```bash
dotnet add package MCPnet
```

## Usage

### Basic Setup

```csharp
// Add the MCPnet services to your DI container
services.AddMcpServices();

// Or configure them individually
services.AddMcpServices(
    registryBaseUrl: "https://api.mcp.net",
    configureMcpClient: builder => builder.ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30);
    }));
```

### Discovering MCP Servers

```csharp
// Get the registry client
var registry = serviceProvider.GetRequiredService<IMcpRegistry>();

// Search for servers related to image processing
var servers = await registry.SearchServersAsync(
    query: "image processing",
    minRating: 4.0);

foreach (var server in servers)
{
    Console.WriteLine($"{server.Name}: {server.Description}");
}
```

### Invoking MCP Capabilities

```csharp
// Get the MCP client
var mcpClient = serviceProvider.GetRequiredService<IMcpClient>();

// Discover capabilities on a server
var capabilities = await mcpClient.DiscoverCapabilitiesAsync(serverUrl);

// Invoke a capability
var request = new McpRequest
{
    CapabilityId = "image-generation",
    Input = new Dictionary<string, object>
    {
        { "prompt", "A beautiful sunset over mountains" },
        { "style", "photorealistic" }
    }
};

var response = await mcpClient.InvokeCapabilityAsync(serverUrl, request);

if (response.Success)
{
    var imageUrl = response.Output["image_url"] as string;
    Console.WriteLine($"Generated image: {imageUrl}");
}
```

### Creating Capability Remixes

```csharp
// Create a remix of capabilities from different servers
var remix = await registry.CreateRemixAsync(
    name: "Advanced Text Processing Suite",
    description: "A collection of the best text processing tools",
    capabilities: new List<(string ServerId, string CapabilityId)>
    {
        ("server1", "text-summarization"),
        ("server2", "sentiment-analysis"),
        ("server3", "language-translation")
    });

Console.WriteLine($"Created remix: {remix.Name} with {remix.Capabilities.Count} capabilities");
```

## Industry-Specific Examples

MCPnet can be applied across various industries to solve domain-specific problems:

### Healthcare

```csharp
// Find medical imaging analysis capabilities
var medicalServers = await mcpRegistry.SearchServersAsync(
    query: "medical imaging analysis radiology",
    minRating: 4.5);

// Example: Process a CT scan for lung nodule detection
var request = new McpRequest
{
    CapabilityId = lungNoduleCapability.Id,
    Input = new Dictionary<string, object>
    {
        { "image_data", ctScanData },
        { "patient_age", 65 },
        { "patient_sex", "male" },
        { "scan_type", "chest_ct" }
    }
};

var response = await mcpClient.InvokeCapabilityAsync(medicalServer.Url, request);
```

### Finance

```csharp
// Create a custom financial analysis toolkit by remixing capabilities
var capabilitiesToRemix = new List<(string ServerId, string CapabilityId)>();

// Add sentiment analysis for financial news
capabilitiesToRemix.Add((sentimentCapabilities[0].Server.Id, sentimentCapabilities[0].Capability.Id));

// Add stock prediction capabilities
capabilitiesToRemix.Add((predictionCapabilities[0].Server.Id, predictionCapabilities[0].Capability.Id));

// Add fraud detection capabilities
capabilitiesToRemix.Add((fraudCapabilities[0].Server.Id, fraudCapabilities[0].Capability.Id));

var financeToolkit = await mcpRegistry.CreateRemixAsync(
    name: "Advanced Financial Analysis Toolkit",
    description: "A comprehensive toolkit for financial analysis, prediction and fraud detection",
    capabilities: capabilitiesToRemix);
```

### Content Creation

```csharp
// Generate images based on a product description
var imageRequest = new McpRequest
{
    CapabilityId = imageCapability.Id,
    Input = new Dictionary<string, object>
    {
        { "prompt", "A sleek modern smartphone with an edge-to-edge display" },
        { "style", "photorealistic product photography" },
        { "aspect_ratio", "16:9" },
        { "count", 3 }
    }
};

// Generate narration audio for promotional content
var audioRequest = new McpRequest
{
    CapabilityId = audioCapability.Id,
    Input = new Dictionary<string, object>
    {
        { "text", "Introducing our latest smartphone..." },
        { "voice", "professional_male_en_us" },
        { "speed", 1.0 }
    }
};

// Combine images and audio into a video
var videoRequest = new McpRequest
{
    CapabilityId = videoCapability.Id,
    Input = new Dictionary<string, object>
    {
        { "images", imageUrls },
        { "audio_track", audioUrl },
        { "duration", 30 }
    }
};
```

## Enterprise Integration Examples

MCPnet integrates seamlessly with enterprise systems:

### Document Processing Workflow

```csharp
// Process an invoice through multiple stages
// Step 1: Convert document to text using OCR
var ocrRequest = new McpRequest
{
    CapabilityId = ocrCapability.Id,
    Input = new Dictionary<string, object>
    {
        { "document_url", invoiceDocument },
        { "output_format", "text" },
        { "language", "en" }
    }
};

// Step 2: Classify the document type
var classificationRequest = new McpRequest
{
    CapabilityId = classificationCapability.Id,
    Input = new Dictionary<string, object>
    {
        { "text", textContent },
        { "possible_types", new[] { "invoice", "purchase_order", "receipt" } }
    }
};

// Step 3: Extract structured data based on document type
var extractionRequest = new McpRequest
{
    CapabilityId = extractionCapability.Id,
    Input = new Dictionary<string, object>
    {
        { "text", textContent },
        { "document_type", documentType },
        { "fields", new[] { "invoice_number", "date", "total_amount" } }
    }
};
```

### IoT Data Analysis

```csharp
// Detect anomalies in sensor data
var anomalyRequest = new McpRequest
{
    CapabilityId = anomalyCapability.Id,
    Input = new Dictionary<string, object>
    {
        { "sensor_data", sensorData },
        { "sensitivity", "medium" }
    }
};

// Predict equipment maintenance needs
var maintenanceRequest = new McpRequest
{
    CapabilityId = maintenanceCapability.Id,
    Input = new Dictionary<string, object>
    {
        { "equipment_id", "PUMP-123" },
        { "sensor_data", sensorData },
        { "anomalies", anomalies }
    }
};
```

### SAP Integration for Supply Chain Management

```csharp
// Generate demand forecasts
var forecastRequest = new McpRequest
{
    CapabilityId = forecastCapability.Id,
    Input = new Dictionary<string, object>
    {
        { "historical_data", historicalSalesData },
        { "forecast_periods", 6 } // 6 months ahead
    }
};

// Optimize inventory based on forecasts
var inventoryRequest = new McpRequest
{
    CapabilityId = inventoryCapability.Id,
    Input = new Dictionary<string, object>
    {
        { "forecasts", forecasts },
        { "current_inventory", currentInventory },
        { "service_level", 0.95 } // 95% service level
    }
};

// Analyze supply chain risks
var riskRequest = new McpRequest
{
    CapabilityId = riskCapability.Id,
    Input = new Dictionary<string, object>
    {
        { "supply_chain_data", supplyChainData },
        { "external_risk_factors", externalRiskFactors }
    }
};
```

## Running the Examples

The library includes a comprehensive set of examples demonstrating how MCPnet can be used in various scenarios. To run them, use the `ExampleRunner` class:

```csharp
// Run all examples
await ExampleRunner.RunAllExamplesAsync();

// Run only industry-specific examples
await ExampleRunner.RunIndustryExamplesAsync();

// Run only enterprise integration examples
await ExampleRunner.RunEnterpriseExamplesAsync();

// Run a specific example
await IndustrySpecificExamples.HealthcareExampleAsync();
await EnterpriseIntegrationExamples.IoTDataAnalysisAsync();
```

See the [Examples](MCPnet.Client/Examples/) directory for more detailed usage examples.

## Repository

The source code for MCPnet is available on GitHub:
[https://github.com/adhammehdawi/MCPnet](https://github.com/adhammehdawi/MCPnet)

## Author

**Adham Mehdawi**
mehdawiadham@gmail.com

## Support

If you find this library useful, consider buying me a coffee:

[![](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)](https://www.buymeacoffee.com/adhammehdawi)

## License

MIT

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
