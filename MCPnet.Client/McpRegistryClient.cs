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
    /// Implementation of the MCP registry client that communicates with the MCPnet registry service
    /// </summary>
    public class McpRegistryClient : IMcpRegistry
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseUrl;

        /// <summary>
        /// Create a new MCP registry client
        /// </summary>
        /// <param name="httpClient">The HttpClient to use for requests</param>
        /// <param name="baseUrl">The base URL of the MCPnet registry service (defaults to https://api.mcp.net)</param>
        public McpRegistryClient(HttpClient httpClient, string baseUrl = "https://api.mcp.net")
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _baseUrl = string.IsNullOrEmpty(baseUrl) ? "https://api.mcp.net" : baseUrl;
            
            // Ensure the URL ends with a slash
            if (!_baseUrl.EndsWith("/"))
            {
                _baseUrl += "/";
            }

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        /// <inheritdoc />
        public async Task<List<McpServer>> SearchServersAsync(
            string query = null,
            List<string> tags = null,
            double? minRating = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var requestUrl = $"{_baseUrl}servers?page={page}&pageSize={pageSize}";
            
            if (!string.IsNullOrEmpty(query))
            {
                requestUrl += $"&query={Uri.EscapeDataString(query)}";
            }
            
            if (minRating.HasValue)
            {
                requestUrl += $"&minRating={minRating.Value}";
            }
            
            if (tags != null && tags.Count > 0)
            {
                requestUrl += $"&tags={Uri.EscapeDataString(string.Join(",", tags))}";
            }

            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<List<McpServer>>(_jsonOptions, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<McpServer> GetServerDetailsAsync(
            string serverId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new ArgumentException("Server ID must be provided", nameof(serverId));

            var response = await _httpClient.GetAsync($"{_baseUrl}servers/{serverId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<McpServer>(_jsonOptions, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<McpServer> SubmitServerAsync(
            McpServer server,
            CancellationToken cancellationToken = default)
        {
            if (server == null)
                throw new ArgumentNullException(nameof(server));

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}servers", server, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<McpServer>(_jsonOptions, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<McpReview> SubmitRatingAsync(
            string serverId,
            double score,
            string reviewText = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(serverId))
                throw new ArgumentException("Server ID must be provided", nameof(serverId));

            if (score < 0 || score > 5)
                throw new ArgumentException("Rating score must be between 0 and 5", nameof(score));

            var review = new
            {
                serverId,
                score,
                text = reviewText
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}servers/{serverId}/ratings", 
                review, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<McpReview>(_jsonOptions, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<List<(McpCapability Capability, McpServer Server)>> SearchCapabilitiesAsync(
            string query,
            double? minRating = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(query))
                throw new ArgumentException("Search query must be provided", nameof(query));

            var requestUrl = $"{_baseUrl}capabilities/search?query={Uri.EscapeDataString(query)}&page={page}&pageSize={pageSize}";
            
            if (minRating.HasValue)
            {
                requestUrl += $"&minRating={minRating.Value}";
            }

            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            // This result is a bit complex to deserialize directly to the tuple,
            // so we'll use an intermediate DTO
            var results = await response.Content.ReadFromJsonAsync<List<CapabilitySearchResult>>(_jsonOptions, cancellationToken);
            
            var tupleResults = new List<(McpCapability Capability, McpServer Server)>();
            foreach (var result in results)
            {
                tupleResults.Add((result.Capability, result.Server));
            }
            
            return tupleResults;
        }

        /// <inheritdoc />
        public async Task<McpServer> CreateRemixAsync(
            string name,
            string description,
            List<(string ServerId, string CapabilityId)> capabilities,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Remix name must be provided", nameof(name));
            
            if (capabilities == null || capabilities.Count == 0)
                throw new ArgumentException("At least one capability must be included in the remix", nameof(capabilities));

            // Convert the capabilities list to a format that can be serialized
            var capabilitiesList = new List<Dictionary<string, string>>();
            foreach (var capability in capabilities)
            {
                capabilitiesList.Add(new Dictionary<string, string>
                {
                    { "serverId", capability.ServerId },
                    { "capabilityId", capability.CapabilityId }
                });
            }

            var remix = new
            {
                name,
                description,
                capabilities = capabilitiesList
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}remixes", 
                remix, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<McpServer>(_jsonOptions, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<McpFunctionalityRequest> RequestFunctionalityAsync(
            string description,
            string useCase,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Description must be provided", nameof(description));

            var request = new
            {
                description,
                useCase
            };

            var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}requests", 
                request, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<McpFunctionalityRequest>(_jsonOptions, cancellationToken);
        }

        // Helper DTO class for capability search results
        private class CapabilitySearchResult
        {
            public McpCapability Capability { get; set; }
            public McpServer Server { get; set; }
        }
    }
} 