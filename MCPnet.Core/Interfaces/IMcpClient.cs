using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MCPnet.Core.Models;

namespace MCPnet.Core.Interfaces
{
    /// <summary>
    /// Interface for an MCP client that can communicate with MCP servers
    /// </summary>
    public interface IMcpClient
    {
        /// <summary>
        /// Discover the capabilities of an MCP server
        /// </summary>
        /// <param name="serverUrl">The base URL of the MCP server</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A list of capabilities provided by the server</returns>
        Task<List<McpCapability>> DiscoverCapabilitiesAsync(string serverUrl, CancellationToken cancellationToken = default);

        /// <summary>
        /// Invoke a capability on an MCP server
        /// </summary>
        /// <param name="serverUrl">The base URL of the MCP server</param>
        /// <param name="request">The request details</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>The response from the capability invocation</returns>
        Task<McpResponse> InvokeCapabilityAsync(string serverUrl, McpRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get detailed information about a specific capability
        /// </summary>
        /// <param name="serverUrl">The base URL of the MCP server</param>
        /// <param name="capabilityId">The ID of the capability to get details for</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Detailed information about the capability</returns>
        Task<McpCapability> GetCapabilityDetailsAsync(string serverUrl, string capabilityId, CancellationToken cancellationToken = default);
    }
} 