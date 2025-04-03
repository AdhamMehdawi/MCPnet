using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MCPnet.Core.Models;

namespace MCPnet.Core.Interfaces
{
    /// <summary>
    /// Interface for the MCPnet registry service that manages MCP servers
    /// </summary>
    public interface IMcpRegistry
    {
        /// <summary>
        /// Search for MCP servers based on various criteria
        /// </summary>
        /// <param name="query">Optional text query for semantic search</param>
        /// <param name="tags">Optional tags to filter by</param>
        /// <param name="minRating">Optional minimum rating threshold</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of results per page</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A list of matching MCP servers</returns>
        Task<List<McpServer>> SearchServersAsync(
            string query = null,
            List<string> tags = null,
            double? minRating = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get detailed information about a specific MCP server
        /// </summary>
        /// <param name="serverId">The ID of the server to get details for</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>Detailed information about the server</returns>
        Task<McpServer> GetServerDetailsAsync(string serverId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Submit a new MCP server to the registry
        /// </summary>
        /// <param name="server">The server information to submit</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>The created server with its assigned ID</returns>
        Task<McpServer> SubmitServerAsync(McpServer server, CancellationToken cancellationToken = default);

        /// <summary>
        /// Submit a rating and review for an MCP server
        /// </summary>
        /// <param name="serverId">The ID of the server to rate</param>
        /// <param name="score">Rating score (typically 0-5)</param>
        /// <param name="reviewText">Optional review text</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>The submitted review</returns>
        Task<McpReview> SubmitRatingAsync(
            string serverId,
            double score,
            string reviewText = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Search for capabilities across all registered servers
        /// </summary>
        /// <param name="query">Text query for semantic search of capabilities</param>
        /// <param name="minRating">Optional minimum server rating threshold</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of results per page</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>A list of matching capabilities with their server information</returns>
        Task<List<(McpCapability Capability, McpServer Server)>> SearchCapabilitiesAsync(
            string query,
            double? minRating = null,
            int page = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a server "remix" that combines tools from different servers
        /// </summary>
        /// <param name="name">Name for the remix</param>
        /// <param name="description">Description of the remix</param>
        /// <param name="capabilities">List of capability IDs from different servers to include</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>The created remix server</returns>
        Task<McpServer> CreateRemixAsync(
            string name,
            string description,
            List<(string ServerId, string CapabilityId)> capabilities,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Submit a request for new functionality
        /// </summary>
        /// <param name="description">Description of the requested functionality</param>
        /// <param name="useCase">The use case or scenario where this would be helpful</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>The submitted request with its ID</returns>
        Task<McpFunctionalityRequest> RequestFunctionalityAsync(
            string description,
            string useCase,
            CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// A request for new functionality to be added to the MCPnet network
    /// </summary>
    public class McpFunctionalityRequest
    {
        /// <summary>
        /// The unique identifier for this request
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Description of the requested functionality
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The use case or scenario where this would be helpful
        /// </summary>
        public string UseCase { get; set; }

        /// <summary>
        /// The current status of the request
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// When the request was created
        /// </summary>
        public string CreatedAt { get; set; }

        /// <summary>
        /// Vote count showing community interest
        /// </summary>
        public int Votes { get; set; }
    }
} 