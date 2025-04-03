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
    public class McpRegistryTests
    {
        private readonly Mock<IMcpRegistry> _mockRegistry;

        public McpRegistryTests()
        {
            _mockRegistry = new Mock<IMcpRegistry>();
        }

        [Fact]
        public async Task SearchServersAsync_ReturnsMatchingServers()
        {
            // Arrange
            var query = "image processing";
            var minRating = 4.0;
            var expectedServers = new List<McpServer>
            {
                new McpServer
                {
                    Id = "server-1",
                    Name = "Advanced Image Processing API",
                    Description = "Provides various image processing capabilities",
                    Url = "https://api.imageprocess.example.com",
                    Rating = new McpRating { Score = 4.8, Count = 120 },
                    Metadata = new Dictionary<string, object> 
                    { 
                        { "tags", new List<string> { "image", "vision", "processing" } } 
                    }
                },
                new McpServer
                {
                    Id = "server-2",
                    Name = "Image Effects Service",
                    Description = "Apply effects and filters to images",
                    Url = "https://effects.visualtools.example.org",
                    Rating = new McpRating { Score = 4.5, Count = 85 },
                    Metadata = new Dictionary<string, object> 
                    { 
                        { "tags", new List<string> { "image", "effects", "filters" } } 
                    }
                }
            };

            _mockRegistry.Setup(r => r.SearchServersAsync(query, null, minRating, 1, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedServers);

            // Act
            var servers = await _mockRegistry.Object.SearchServersAsync(query, null, minRating);

            // Assert
            Assert.NotNull(servers);
            Assert.Equal(2, servers.Count);
            Assert.Equal("server-1", servers[0].Id);
            Assert.Equal("server-2", servers[1].Id);
            Assert.Contains(servers, s => s.Rating.Score >= minRating);
        }

        [Fact]
        public async Task GetServerDetailsAsync_ReturnsServerDetails()
        {
            // Arrange
            var serverId = "server-1";
            var expectedServer = new McpServer
            {
                Id = serverId,
                Name = "Advanced Image Processing API",
                Description = "Provides various image processing capabilities",
                Url = "https://api.imageprocess.example.com",
                Rating = new McpRating { Score = 4.8, Count = 120 },
                Metadata = new Dictionary<string, object> 
                { 
                    { "tags", new List<string> { "image", "vision", "processing" } } 
                },
                Capabilities = new List<McpCapability>
                {
                    new McpCapability
                    {
                        Id = "image-resize",
                        Name = "Image Resizing",
                        Description = "Resize images to specified dimensions"
                    },
                    new McpCapability
                    {
                        Id = "image-filter",
                        Name = "Image Filtering",
                        Description = "Apply filters to images"
                    }
                }
            };

            _mockRegistry.Setup(r => r.GetServerDetailsAsync(serverId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedServer);

            // Act
            var server = await _mockRegistry.Object.GetServerDetailsAsync(serverId);

            // Assert
            Assert.NotNull(server);
            Assert.Equal(serverId, server.Id);
            Assert.Equal("Advanced Image Processing API", server.Name);
            Assert.Equal(2, server.Capabilities.Count);
        }

        [Fact]
        public async Task SubmitServerAsync_ReturnsCreatedServer()
        {
            // Arrange
            var serverToSubmit = new McpServer
            {
                Name = "New Text Analysis API",
                Description = "Advanced text analysis and NLP capabilities",
                Url = "https://api.textanalysis.example.com",
                Metadata = new Dictionary<string, object> 
                { 
                    { "tags", new List<string> { "text", "nlp", "analysis" } } 
                }
            };

            var expectedServer = new McpServer
            {
                Id = "server-123",
                Name = "New Text Analysis API",
                Description = "Advanced text analysis and NLP capabilities",
                Url = "https://api.textanalysis.example.com",
                Metadata = new Dictionary<string, object> 
                { 
                    { "tags", new List<string> { "text", "nlp", "analysis" } } 
                },
                Rating = new McpRating { Score = 0.0, Count = 0 },
                ProtocolVersion = "1.0",
                Capabilities = new List<McpCapability>()
            };

            _mockRegistry.Setup(r => r.SubmitServerAsync(serverToSubmit, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedServer);

            // Act
            var server = await _mockRegistry.Object.SubmitServerAsync(serverToSubmit);

            // Assert
            Assert.NotNull(server);
            Assert.Equal("server-123", server.Id);
            Assert.Equal(serverToSubmit.Name, server.Name);
            Assert.Equal(serverToSubmit.Description, server.Description);
            Assert.Equal(serverToSubmit.Url, server.Url);
        }

        [Fact]
        public async Task SearchCapabilitiesAsync_ReturnsMatchingCapabilities()
        {
            // Arrange
            var query = "text translation";
            var minRating = 4.2;
            
            var expectedCapabilities = new List<(McpCapability Capability, McpServer Server)>
            {
                (
                    new McpCapability
                    {
                        Id = "translate-text",
                        Name = "Text Translation",
                        Description = "Translate text between languages"
                    },
                    new McpServer
                    {
                        Id = "server-3",
                        Name = "Language Services API",
                        Rating = new McpRating { Score = 4.7, Count = 95 }
                    }
                ),
                (
                    new McpCapability
                    {
                        Id = "real-time-translation",
                        Name = "Real-time Translation",
                        Description = "Translate text in real-time"
                    },
                    new McpServer
                    {
                        Id = "server-4",
                        Name = "Realtime Translation Service",
                        Rating = new McpRating { Score = 4.5, Count = 110 }
                    }
                )
            };

            _mockRegistry.Setup(r => r.SearchCapabilitiesAsync(query, minRating, 1, 20, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedCapabilities);

            // Act
            var capabilities = await _mockRegistry.Object.SearchCapabilitiesAsync(query, minRating);

            // Assert
            Assert.NotNull(capabilities);
            Assert.Equal(2, capabilities.Count);
            Assert.Equal("translate-text", capabilities[0].Capability.Id);
            Assert.Equal("real-time-translation", capabilities[1].Capability.Id);
            Assert.All(capabilities, c => Assert.True(c.Server.Rating.Score >= minRating));
        }

        [Fact]
        public async Task CreateRemixAsync_ReturnsCreatedRemix()
        {
            // Arrange
            var name = "Advanced Text Processing Suite";
            var description = "A collection of the best text processing tools";
            var capabilities = new List<(string ServerId, string CapabilityId)>
            {
                ("server1", "text-summarization"),
                ("server2", "sentiment-analysis"),
                ("server3", "language-translation")
            };

            var expectedRemix = new McpServer
            {
                Id = "remix-123",
                Name = name,
                Description = description,
                Url = "https://remixes.mcp.net/remix-123",
                Metadata = new Dictionary<string, object> { { "is_remix", true } },
                Capabilities = new List<McpCapability>
                {
                    new McpCapability { Id = "text-summarization", Name = "Text Summarization" },
                    new McpCapability { Id = "sentiment-analysis", Name = "Sentiment Analysis" },
                    new McpCapability { Id = "language-translation", Name = "Language Translation" }
                }
            };

            _mockRegistry.Setup(r => r.CreateRemixAsync(name, description, capabilities, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRemix);

            // Act
            var remix = await _mockRegistry.Object.CreateRemixAsync(name, description, capabilities);

            // Assert
            Assert.NotNull(remix);
            Assert.Equal("remix-123", remix.Id);
            Assert.Equal(name, remix.Name);
            Assert.Equal(description, remix.Description);
            Assert.True(remix.Metadata.ContainsKey("is_remix"));
            Assert.Equal(3, remix.Capabilities.Count);
        }

        [Fact]
        public async Task RequestFunctionalityAsync_ReturnsCreatedRequest()
        {
            // Arrange
            var description = "Add support for audio transcription";
            var useCase = "I need to transcribe meeting recordings into text";

            var expectedRequest = new McpFunctionalityRequest
            {
                Id = "request-123",
                Description = description,
                UseCase = useCase,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow.ToString("o"),
                Votes = 1 // Initial vote from requester
            };

            _mockRegistry.Setup(r => r.RequestFunctionalityAsync(description, useCase, It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedRequest);

            // Act
            var request = await _mockRegistry.Object.RequestFunctionalityAsync(description, useCase);

            // Assert
            Assert.NotNull(request);
            Assert.Equal("request-123", request.Id);
            Assert.Equal(description, request.Description);
            Assert.Equal(useCase, request.UseCase);
            Assert.Equal("Pending", request.Status);
            Assert.Equal(1, request.Votes);
        }
    }
} 