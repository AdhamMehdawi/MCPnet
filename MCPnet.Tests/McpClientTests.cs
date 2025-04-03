using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MCPnet.Core.Interfaces;
using MCPnet.Core.Models;
using Moq;
using Xunit;

namespace MCPnet.Tests
{
    public class McpClientTests
    {
        private readonly Mock<IMcpClient> _mockClient;
        private readonly string _testServerUrl = "https://test-server.mcp.net";

        public McpClientTests()
        {
            _mockClient = new Mock<IMcpClient>();
        }

        [Fact]
        public async Task DiscoverCapabilitiesAsync_ReturnsCapabilities()
        {
            // Arrange
            var expectedCapabilities = new List<McpCapability>
            {
                new McpCapability
                {
                    Id = "image-generation",
                    Name = "Image Generation",
                    Description = "Generates images from text prompts",
                    InputSchema = new Dictionary<string, object>
                    {
                        { "prompt", new { type = "string", description = "Text prompt for image generation" } }
                    },
                    OutputSchema = new Dictionary<string, object>
                    {
                        { "image_url", new { type = "string", description = "URL to the generated image" } }
                    }
                },
                new McpCapability
                {
                    Id = "text-summarization",
                    Name = "Text Summarization",
                    Description = "Summarizes long text content",
                    InputSchema = new Dictionary<string, object>
                    {
                        { "text", new { type = "string", description = "Long text to summarize" } }
                    },
                    OutputSchema = new Dictionary<string, object>
                    {
                        { "summary", new { type = "string", description = "Summarized text" } }
                    }
                }
            };

            _mockClient.Setup(c => c.DiscoverCapabilitiesAsync(_testServerUrl, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCapabilities);

            // Act
            var capabilities = await _mockClient.Object.DiscoverCapabilitiesAsync(_testServerUrl);

            // Assert
            Assert.NotNull(capabilities);
            Assert.Equal(2, capabilities.Count);
            Assert.Equal("image-generation", capabilities[0].Id);
            Assert.Equal("text-summarization", capabilities[1].Id);
        }

        [Fact]
        public async Task InvokeCapabilityAsync_ReturnsSuccessfulResponse()
        {
            // Arrange
            var request = new McpRequest
            {
                CapabilityId = "image-generation",
                Input = new Dictionary<string, object>
                {
                    { "prompt", "A beautiful sunset over mountains" },
                    { "style", "photorealistic" }
                }
            };

            var expectedResponse = new McpResponse
            {
                Success = true,
                Output = new Dictionary<string, object>
                {
                    { "image_url", "https://example.com/images/generated-12345.jpg" }
                }
            };

            _mockClient.Setup(c => c.InvokeCapabilityAsync(_testServerUrl, request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _mockClient.Object.InvokeCapabilityAsync(_testServerUrl, request);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.NotNull(response.Output);
            Assert.Equal("https://example.com/images/generated-12345.jpg", response.Output["image_url"]);
        }

        [Fact]
        public async Task GetCapabilityDetailsAsync_ReturnsCapabilityDetails()
        {
            // Arrange
            var capabilityId = "image-generation";
            var expectedCapability = new McpCapability
            {
                Id = capabilityId,
                Name = "Image Generation",
                Description = "Generates images from text prompts",
                InputSchema = new Dictionary<string, object>
                {
                    { "prompt", new { type = "string", description = "Text prompt for image generation" } }
                },
                OutputSchema = new Dictionary<string, object>
                {
                    { "image_url", new { type = "string", description = "URL to the generated image" } }
                }
            };

            _mockClient.Setup(c => c.GetCapabilityDetailsAsync(_testServerUrl, capabilityId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCapability);

            // Act
            var capability = await _mockClient.Object.GetCapabilityDetailsAsync(_testServerUrl, capabilityId);

            // Assert
            Assert.NotNull(capability);
            Assert.Equal(capabilityId, capability.Id);
            Assert.Equal("Image Generation", capability.Name);
            Assert.NotNull(capability.InputSchema);
            Assert.NotNull(capability.OutputSchema);
        }
    }
} 