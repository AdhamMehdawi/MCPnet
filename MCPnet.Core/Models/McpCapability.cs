using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MCPnet.Core.Models
{
    /// <summary>
    /// Represents a capability that an MCP server provides
    /// </summary>
    public class McpCapability
    {
        /// <summary>
        /// The unique identifier for this capability
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }

        /// <summary>
        /// The name of the capability
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// A description of what this capability does
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>
        /// The input schema for this capability in JSON Schema format
        /// </summary>
        [JsonPropertyName("input_schema")]
        public Dictionary<string, object> InputSchema { get; set; }

        /// <summary>
        /// The output schema for this capability in JSON Schema format
        /// </summary>
        [JsonPropertyName("output_schema")]
        public Dictionary<string, object> OutputSchema { get; set; }

        /// <summary>
        /// Optional authentication requirements for using this capability
        /// </summary>
        [JsonPropertyName("authentication")]
        public McpAuthentication Authentication { get; set; }

        /// <summary>
        /// Additional metadata for the capability
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }
    }

    /// <summary>
    /// Authentication requirements for an MCP capability
    /// </summary>
    public class McpAuthentication
    {
        /// <summary>
        /// The type of authentication required (e.g., "api_key", "oauth", etc.)
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; }

        /// <summary>
        /// Additional authentication parameters
        /// </summary>
        [JsonPropertyName("parameters")]
        public Dictionary<string, object> Parameters { get; set; }
    }
} 