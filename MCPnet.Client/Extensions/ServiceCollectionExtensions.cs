using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net.Http;
using MCPnet.Core.Interfaces;

namespace MCPnet.Client.Extensions
{
    /// <summary>
    /// Extension methods for IServiceCollection to register MCP services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the MCP client to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configureClient">Optional action to configure the HttpClient</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMcpClient(
            this IServiceCollection services,
            Action<IHttpClientBuilder> configureClient = null)
        {
            var builder = services.AddHttpClient<IMcpClient, McpClient>();
            configureClient?.Invoke(builder);
            
            return services;
        }

        /// <summary>
        /// Add the MCP registry client to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="baseUrl">The base URL of the MCPnet registry service (defaults to https://api.mcp.net)</param>
        /// <param name="configureClient">Optional action to configure the HttpClient</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMcpRegistry(
            this IServiceCollection services,
            string baseUrl = "https://api.mcp.net",
            Action<IHttpClientBuilder> configureClient = null)
        {
            var builder = services.AddHttpClient<IMcpRegistry, McpRegistryClient>(client =>
            {
                // Configure the client to use the specified base URL
                // The actual URL handling is done in the McpRegistryClient constructor
            });
            
            configureClient?.Invoke(builder);
            
            // Register the registry client with the specified base URL
            services.AddTransient<IMcpRegistry>(provider =>
            {
                var httpClientFactory = provider.GetRequiredService<IHttpClientFactory>();
                var httpClient = httpClientFactory.CreateClient(nameof(McpRegistryClient));
                return new McpRegistryClient(httpClient, baseUrl);
            });
            
            return services;
        }

        /// <summary>
        /// Add both the MCP client and registry client to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="registryBaseUrl">The base URL of the MCPnet registry service (defaults to https://api.mcp.net)</param>
        /// <param name="configureMcpClient">Optional action to configure the MCP client's HttpClient</param>
        /// <param name="configureRegistryClient">Optional action to configure the registry client's HttpClient</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddMcpServices(
            this IServiceCollection services,
            string registryBaseUrl = "https://api.mcp.net",
            Action<IHttpClientBuilder> configureMcpClient = null,
            Action<IHttpClientBuilder> configureRegistryClient = null)
        {
            services.AddMcpClient(configureMcpClient);
            services.AddMcpRegistry(registryBaseUrl, configureRegistryClient);
            
            return services;
        }
    }
} 