using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MCPnet.Core.Models
{
    /// <summary>
    /// Represents an MCP server in the MCPnet network
    /// </summary>
    public class McpServer
    {
        /// <summary>
        /// The unique identifier for this server
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the server
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// A description of what this server provides
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The base URL of the server
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }

        /// <summary>
        /// The capabilities provided by this server
        /// </summary>
        [JsonPropertyName("capabilities")]
        public List<McpCapability> Capabilities { get; set; }

        /// <summary>
        /// The version of the MCP protocol supported by this server
        /// </summary>
        [JsonPropertyName("protocol_version")]
        public string ProtocolVersion { get; set; }

        /// <summary>
        /// Optional contact information for the server maintainer
        /// </summary>
        [JsonPropertyName("contact")]
        public McpContact Contact { get; set; }

        /// <summary>
        /// Rating/review information for this server
        /// </summary>
        [JsonPropertyName("rating")]
        public McpRating Rating { get; set; }

        /// <summary>
        /// Additional metadata for the server
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }
    }

    /// <summary>
    /// Contact information for an MCP server maintainer
    /// </summary>
    public class McpContact
    {
        /// <summary>
        /// The name of the maintainer or organization
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The email address of the maintainer
        /// </summary>
        [JsonPropertyName("email")]
        public string Email { get; set; }

        /// <summary>
        /// The URL of the maintainer's website
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    /// <summary>
    /// Rating and review information for an MCP server
    /// </summary>
    public class McpRating
    {
        /// <summary>
        /// The average rating score (typically 0-5)
        /// </summary>
        [JsonPropertyName("score")]
        public double Score { get; set; }

        /// <summary>
        /// The number of ratings received
        /// </summary>
        [JsonPropertyName("count")]
        public int Count { get; set; }

        /// <summary>
        /// List of most recent or highlighted reviews
        /// </summary>
        [JsonPropertyName("recent_reviews")]
        public List<McpReview> RecentReviews { get; set; }
    }

    /// <summary>
    /// A user review of an MCP server
    /// </summary>
    public class McpReview
    {
        /// <summary>
        /// The unique identifier for this review
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The user who provided the review
        /// </summary>
        [JsonPropertyName("user")]
        public string User { get; set; }

        /// <summary>
        /// The rating score (typically 0-5)
        /// </summary>
        [JsonPropertyName("score")]
        public double Score { get; set; }

        /// <summary>
        /// The review text
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>
        /// When the review was created
        /// </summary>
        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }
    }
} 