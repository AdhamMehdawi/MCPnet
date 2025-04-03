using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MCPnet.Core.Interfaces;
using MCPnet.Core.Models;

namespace MCPnet.Client
{
    /// <summary>
    /// Implementation of the MCP client that communicates with MCP servers
    /// </summary>
    public class McpClient : IMcpClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// Create a new MCP client
        /// </summary>
        /// <param name="httpClient">The HttpClient to use for requests</param>
        public McpClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <inheritdoc />
        public async Task<List<McpCapability>> DiscoverCapabilitiesAsync(
            string serverUrl,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(serverUrl))
                throw new ArgumentException("Server URL must be provided", nameof(serverUrl));

            // Ensure the URL ends with a slash
            var baseUrl = serverUrl.EndsWith("/") ? serverUrl : serverUrl + "/";
            var response = await _httpClient.GetAsync($"{baseUrl}capabilities", cancellationToken);
            
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<McpCapability>>(_jsonOptions, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<McpCapability> GetCapabilityDetailsAsync(
            string serverUrl,
            string capabilityId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(serverUrl))
                throw new ArgumentException("Server URL must be provided", nameof(serverUrl));
            
            if (string.IsNullOrEmpty(capabilityId))
                throw new ArgumentException("Capability ID must be provided", nameof(capabilityId));

            // Ensure the URL ends with a slash
            var baseUrl = serverUrl.EndsWith("/") ? serverUrl : serverUrl + "/";
            var response = await _httpClient.GetAsync($"{baseUrl}capabilities/{capabilityId}", cancellationToken);
            
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<McpCapability>(_jsonOptions, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<McpResponse> InvokeCapabilityAsync(
            string serverUrl,
            McpRequest request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(serverUrl))
                throw new ArgumentException("Server URL must be provided", nameof(serverUrl));
            
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            
            if (string.IsNullOrEmpty(request.CapabilityId))
                throw new ArgumentException("Capability ID must be provided in the request", nameof(request));

            // Generate a request ID if not provided
            if (string.IsNullOrEmpty(request.RequestId))
            {
                request.RequestId = Guid.NewGuid().ToString("N");
            }

            // Ensure the URL ends with a slash
            var baseUrl = serverUrl.EndsWith("/") ? serverUrl : serverUrl + "/";
            var requestUrl = $"{baseUrl}invoke/{request.CapabilityId}";
            
            var response = await _httpClient.PostAsJsonAsync(requestUrl, request, _jsonOptions, cancellationToken);
            
            // Even if the request failed, we want to return the response
            // The success flag in the response will indicate if it was successful
            var mcpResponse = await response.Content.ReadFromJsonAsync<McpResponse>(_jsonOptions, cancellationToken);
            
            // If we couldn't deserialize the response, create an error response
            if (mcpResponse == null)
            {
                mcpResponse = new McpResponse
                {
                    RequestId = request.RequestId,
                    Success = false,
                    Error = new McpError
                    {
                        Code = "client_error",
                        Message = $"Failed to deserialize response from server. Status code: {response.StatusCode}"
                    }
                };
            }
            
            return mcpResponse;
        }
    }
} 