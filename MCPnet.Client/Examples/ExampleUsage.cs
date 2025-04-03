using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MCPnet.Client.Extensions;
using MCPnet.Core.Interfaces;
using MCPnet.Core.Models;

namespace MCPnet.Client.Examples
{
    /// <summary>
    /// Examples showing how to use the MCPnet library
    /// </summary>
    public static class ExampleUsage
    {
        /// <summary>
        /// Example of how to set up and use the MCP client and registry
        /// </summary>
        public static async Task RunExampleAsync()
        {
            // Set up the services
            var services = new ServiceCollection();
            
            // Add the MCP services with default configuration
            services.AddMcpServices();
            
            // Or configure them individually
            services.AddMcpServices(
                registryBaseUrl: "https://api.mcp.net",
                configureMcpClient: builder => builder.ConfigureHttpClient(client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(30);
                }),
                configureRegistryClient: builder => builder.ConfigureHttpClient(client =>
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "MCPnet Example");
                }));
            
            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();
            
            // Get the MCP client and registry
            var mcpClient = serviceProvider.GetRequiredService<IMcpClient>();
            var mcpRegistry = serviceProvider.GetRequiredService<IMcpRegistry>();
            
            // Example: Search for MCP servers related to image processing
            var imageServers = await mcpRegistry.SearchServersAsync(
                query: "image processing",
                minRating: 4.0);
            
            Console.WriteLine($"Found {imageServers.Count} image processing servers:");
            foreach (var server in imageServers)
            {
                Console.WriteLine($"- {server.Name}: {server.Description}");
            }
            
            // Example: Get details for the first server
            if (imageServers.Count > 0)
            {
                var serverId = imageServers[0].Id;
                var serverDetails = await mcpRegistry.GetServerDetailsAsync(serverId);
                
                Console.WriteLine($"\nServer details for {serverDetails.Name}:");
                Console.WriteLine($"URL: {serverDetails.Url}");
                Console.WriteLine($"Rating: {serverDetails.Rating?.Score ?? 0} ({serverDetails.Rating?.Count ?? 0} reviews)");
                Console.WriteLine($"Capabilities: {serverDetails.Capabilities?.Count ?? 0}");
                
                // Example: Discover capabilities directly from the server
                var capabilities = await mcpClient.DiscoverCapabilitiesAsync(serverDetails.Url);
                
                Console.WriteLine("\nCapabilities:");
                foreach (var capability in capabilities)
                {
                    Console.WriteLine($"- {capability.Name}: {capability.Description}");
                }
                
                // Example: Invoke a capability if one exists
                if (capabilities.Count > 0)
                {
                    var capability = capabilities[0];
                    
                    // Prepare the request with appropriate input according to the capability's schema
                    var capabilityRequest = new McpRequest
                    {
                        CapabilityId = capability.Id,
                        Input = new Dictionary<string, object>
                        {
                            // Example input - would need to match the capability's schema
                            { "text", "Hello, world!" }
                        }
                    };
                    
                    // Invoke the capability
                    var response = await mcpClient.InvokeCapabilityAsync(serverDetails.Url, capabilityRequest);
                    
                    if (response.Success)
                    {
                        Console.WriteLine("\nCapability invocation successful!");
                        Console.WriteLine("Output:");
                        foreach (var kvp in response.Output)
                        {
                            Console.WriteLine($"- {kvp.Key}: {kvp.Value}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\nCapability invocation failed: {response.Error.Message}");
                    }
                }
            }
            
            // Example: Search for capabilities across all servers
            var textCapabilities = await mcpRegistry.SearchCapabilitiesAsync(
                query: "text generation",
                minRating: 4.5);
            
            Console.WriteLine($"\nFound {textCapabilities.Count} text generation capabilities:");
            foreach (var (capability, server) in textCapabilities)
            {
                Console.WriteLine($"- {capability.Name} (from {server.Name}): {capability.Description}");
            }
            
            // Example: Create a remix of capabilities
            if (textCapabilities.Count > 0)
            {
                var capabilitiesToRemix = new List<(string ServerId, string CapabilityId)>();
                foreach (var (capability, server) in textCapabilities.GetRange(0, Math.Min(3, textCapabilities.Count)))
                {
                    capabilitiesToRemix.Add((server.Id, capability.Id));
                }
                
                var remix = await mcpRegistry.CreateRemixAsync(
                    name: "My Text Processing Remix",
                    description: "A remix of the best text processing capabilities",
                    capabilities: capabilitiesToRemix);
                
                Console.WriteLine($"\nCreated remix: {remix.Name} with {remix.Capabilities.Count} capabilities");
            }
            
            // Example: Submit a request for new functionality
            var functionalityRequest = await mcpRegistry.RequestFunctionalityAsync(
                description: "Real-time collaboration on document editing",
                useCase: "I want to collaborate with my team on editing documents in real-time");
            
            Console.WriteLine($"\nSubmitted functionality request: {functionalityRequest.Id}");
            Console.WriteLine($"Status: {functionalityRequest.Status}");
        }
    }
} 