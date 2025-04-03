using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MCPnet.Client.Examples;
using MCPnet.Core.Interfaces;
using MCPnet.Core.Models;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace MCPnet.Tests
{
    public class ExampleTests
    {
        private readonly Mock<IMcpClient> _mockClient;
        private readonly Mock<IMcpRegistry> _mockRegistry;
        private readonly ServiceProvider _serviceProvider;

        public ExampleTests()
        {
            _mockClient = new Mock<IMcpClient>();
            _mockRegistry = new Mock<IMcpRegistry>();
            
            // Set up the service collection with mocked dependencies
            var services = new ServiceCollection();
            services.AddSingleton(_mockClient.Object);
            services.AddSingleton(_mockRegistry.Object);
            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact(Skip = "Integration test - requires real services")]
        public async Task RunExampleAsync_ExecutesSuccessfully()
        {
            // This is an integration test that would connect to real services
            // It's skipped by default to avoid external dependencies in unit tests
            await ExampleUsage.RunExampleAsync();
        }
        
        [Fact]
        public async Task ExampleRunner_ExecutesAllExamples()
        {
            // This test verifies that the ExampleRunner can execute all examples
            // We'll just ensure it doesn't throw exceptions
            
            // Set up minimal mocks to prevent actual network calls
            SetupMinimalMocks();
            
            // Act & Assert - should not throw
            await ExampleRunner.RunAllExamplesAsync();
            // If we get here without an exception, the test passes
        }
        
        [Fact]
        public async Task HealthcareExample_ReturnsExpectedResults()
        {
            // Arrange
            SetupMocksForHealthcareExample();
            
            // Act - should not throw
            await IndustrySpecificExamples.HealthcareExampleAsync();
            
            // Assert
            _mockRegistry.Verify(r => r.SearchServersAsync(
                It.Is<string>(q => q.Contains("medical") && q.Contains("imaging")),
                It.IsAny<List<string>>(),
                It.IsAny<double?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            
            _mockClient.Verify(c => c.DiscoverCapabilitiesAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }
        
        [Fact]
        public async Task DocumentProcessingExample_ProcessesDocumentSuccessfully()
        {
            // Arrange
            SetupMocksForDocumentProcessingExample();
            
            // Act - should not throw
            await EnterpriseIntegrationExamples.DocumentProcessingWorkflowAsync();
            
            // Assert
            _mockRegistry.Verify(r => r.SearchCapabilitiesAsync(
                It.Is<string>(q => q.Contains("OCR")),
                It.IsAny<double?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            
            _mockRegistry.Verify(r => r.SearchCapabilitiesAsync(
                It.Is<string>(q => q.Contains("classification")),
                It.IsAny<double?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
            
            _mockRegistry.Verify(r => r.SearchCapabilitiesAsync(
                It.Is<string>(q => q.Contains("extraction")),
                It.IsAny<double?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }
        
        private void SetupMinimalMocks()
        {
            // Set up mock for SearchServersAsync
            _mockRegistry.Setup(r => r.SearchServersAsync(
                It.IsAny<string>(),
                It.IsAny<List<string>>(),
                It.IsAny<double?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new List<McpServer> { CreateMockServer() });
            
            // Set up mock for SearchCapabilitiesAsync
            var capabilityServerPair = (Capability: CreateMockCapability("test-capability"), Server: CreateMockServer());
            _mockRegistry.Setup(r => r.SearchCapabilitiesAsync(
                It.IsAny<string>(),
                It.IsAny<double?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new List<(McpCapability Capability, McpServer Server)> { capabilityServerPair });
            
            // Set up mock for DiscoverCapabilitiesAsync
            _mockClient.Setup(c => c.DiscoverCapabilitiesAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new List<McpCapability> { CreateMockCapability("test-capability") });
            
            // Set up mock for InvokeCapabilityAsync
            _mockClient.Setup(c => c.InvokeCapabilityAsync(
                It.IsAny<string>(),
                It.IsAny<McpRequest>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new McpResponse { 
                Success = true,
                Output = new Dictionary<string, object> { 
                    { "result", "Success" },
                    { "confidence", 0.95 },
                    { "text", "Sample text output" },
                    { "image_url", "https://example.com/image.jpg" },
                    { "findings", new Dictionary<string, object> {
                        { "nodule_count", 2 },
                        { "malignancy_risk", "low" },
                        { "recommendation", "Follow up in 6 months" }
                    }},
                    { "sentiment", new Dictionary<string, object> {
                        { "overall", "positive" },
                        { "market_impact", "bullish" },
                        { "confidence", 0.87 }
                    }},
                    { "document_type", "invoice" },
                    { "extracted_fields", new Dictionary<string, object> {
                        { "invoice_number", "INV-12345" },
                        { "date", "2025-04-01" },
                        { "due_date", "2025-04-30" },
                        { "total_amount", 1250.00 },
                        { "vendor_name", "Acme Supplies Ltd." }
                    }},
                    { "anomalies", new List<Dictionary<string, object>> {
                        new Dictionary<string, object> {
                            { "sensor_id", "temp-1" },
                            { "timestamp", "2025-04-01T18:00:00Z" },
                            { "value", 90.2 },
                            { "severity", "medium" }
                        }
                    }},
                    { "audio_url", "https://example.com/audio.mp3" },
                    { "video_url", "https://example.com/video.mp4" },
                    { "order_info", new Dictionary<string, object> {
                        { "status", "shipped" },
                        { "expected_delivery", "2025-04-05" },
                        { "carrier", "FedEx" },
                        { "tracking_number", "FDX123456789" }
                    }}
                }
            });
        }
        
        private void SetupMocksForHealthcareExample()
        {
            // Set up specific mocks for healthcare example
            var medicalServers = new List<McpServer> {
                new McpServer {
                    Id = "medical-server-1",
                    Name = "Medical Imaging AI",
                    Description = "Advanced medical imaging analysis for healthcare professionals",
                    Url = "https://medical-imaging.example.com",
                    Rating = new McpRating { Score = 4.9, Count = 230 }
                }
            };
            
            _mockRegistry.Setup(r => r.SearchServersAsync(
                It.Is<string>(q => q.Contains("medical") && q.Contains("imaging")),
                It.IsAny<List<string>>(),
                It.IsAny<double?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(medicalServers);
            
            var capabilities = new List<McpCapability> {
                new McpCapability {
                    Id = "lung-nodule-detection",
                    Name = "Lung Nodule Detection",
                    Description = "Detects and analyzes potential lung nodules in CT scans"
                }
            };
            
            _mockClient.Setup(c => c.DiscoverCapabilitiesAsync(
                medicalServers[0].Url,
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(capabilities);
            
            var response = new McpResponse {
                Success = true,
                Output = new Dictionary<string, object> {
                    { "findings", new Dictionary<string, object> {
                        { "nodule_count", 2 },
                        { "malignancy_risk", "low" },
                        { "recommendation", "Follow up in 6 months" }
                    }}
                }
            };
            
            _mockClient.Setup(c => c.InvokeCapabilityAsync(
                medicalServers[0].Url,
                It.Is<McpRequest>(r => r.CapabilityId == "lung-nodule-detection"),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(response);
        }
        
        private void SetupMocksForDocumentProcessingExample()
        {
            // Set up specific mocks for document processing example
            var ocrServer = new McpServer {
                Id = "ocr-server",
                Name = "OCR Service",
                Url = "https://ocr.example.com",
                Rating = new McpRating { Score = 4.7, Count = 180 }
            };
            
            var ocrCapability = new McpCapability {
                Id = "document-ocr",
                Name = "Document OCR",
                Description = "Extract text from document images"
            };
            
            var ocrCapabilityResult = new List<(McpCapability Capability, McpServer Server)> {
                (Capability: ocrCapability, Server: ocrServer)
            };
            
            var classificationServer = new McpServer {
                Id = "classification-server",
                Name = "Document Classification Service",
                Url = "https://classification.example.com",
                Rating = new McpRating { Score = 4.5, Count = 150 }
            };
            
            var classificationCapability = new McpCapability {
                Id = "document-classification",
                Name = "Document Classification",
                Description = "Classify document types"
            };
            
            var classificationCapabilityResult = new List<(McpCapability Capability, McpServer Server)> {
                (Capability: classificationCapability, Server: classificationServer)
            };
            
            var extractionServer = new McpServer {
                Id = "extraction-server",
                Name = "Data Extraction Service",
                Url = "https://extraction.example.com",
                Rating = new McpRating { Score = 4.6, Count = 160 }
            };
            
            var extractionCapability = new McpCapability {
                Id = "data-extraction",
                Name = "Data Extraction",
                Description = "Extract structured data from documents"
            };
            
            var extractionCapabilityResult = new List<(McpCapability Capability, McpServer Server)> {
                (Capability: extractionCapability, Server: extractionServer)
            };
            
            _mockRegistry.Setup(r => r.SearchCapabilitiesAsync(
                It.Is<string>(q => q.Contains("OCR")),
                It.IsAny<double?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(ocrCapabilityResult);
            
            _mockRegistry.Setup(r => r.SearchCapabilitiesAsync(
                It.Is<string>(q => q.Contains("classification")),
                It.IsAny<double?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(classificationCapabilityResult);
            
            _mockRegistry.Setup(r => r.SearchCapabilitiesAsync(
                It.Is<string>(q => q.Contains("extraction")),
                It.IsAny<double?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(extractionCapabilityResult);
            
            // Set up OCR response
            var ocrResponse = new McpResponse {
                Success = true,
                Output = new Dictionary<string, object> {
                    { "text", "INVOICE\nInvoice #: INV-12345\nDate: 2025-04-01\nVendor: Acme Supplies Ltd.\nAmount: $1,250.00" },
                    { "confidence", 0.95 }
                }
            };
            
            _mockClient.Setup(c => c.InvokeCapabilityAsync(
                ocrServer.Url,
                It.Is<McpRequest>(r => r.CapabilityId == "document-ocr"),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(ocrResponse);
            
            // Set up classification response
            var classificationResponse = new McpResponse {
                Success = true,
                Output = new Dictionary<string, object> {
                    { "document_type", "invoice" },
                    { "confidence", 0.92 }
                }
            };
            
            _mockClient.Setup(c => c.InvokeCapabilityAsync(
                classificationServer.Url,
                It.Is<McpRequest>(r => r.CapabilityId == "document-classification"),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(classificationResponse);
            
            // Set up extraction response
            var extractionResponse = new McpResponse {
                Success = true,
                Output = new Dictionary<string, object> {
                    { "extracted_fields", new Dictionary<string, object> {
                        { "invoice_number", "INV-12345" },
                        { "date", "2025-04-01" },
                        { "due_date", "2025-04-30" },
                        { "total_amount", 1250.00 },
                        { "tax_amount", 200.00 },
                        { "vendor_name", "Acme Supplies Ltd." },
                        { "vendor_address", "123 Supply St, Vendorville, VN 12345" }
                    }}
                }
            };
            
            _mockClient.Setup(c => c.InvokeCapabilityAsync(
                extractionServer.Url,
                It.Is<McpRequest>(r => r.CapabilityId == "data-extraction"),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(extractionResponse);
        }
        
        private McpServer CreateMockServer()
        {
            return new McpServer
            {
                Id = "mock-server-1",
                Name = "Mock Server",
                Description = "A mock server for testing",
                Url = "https://mock.example.com",
                Rating = new McpRating { Score = 4.5, Count = 100 },
                Capabilities = new List<McpCapability>()
            };
        }
        
        private McpCapability CreateMockCapability(string id)
        {
            return new McpCapability
            {
                Id = id,
                Name = "Mock Capability",
                Description = "A mock capability for testing",
                InputSchema = new Dictionary<string, object>(),
                OutputSchema = new Dictionary<string, object>()
            };
        }
    }
} 