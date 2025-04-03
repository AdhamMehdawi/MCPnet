using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MCPnet.Core.Models
{
    /// <summary>
    /// Represents a request to invoke an MCP capability
    /// </summary>
    public class McpRequest
    {
        /// <summary>
        /// A unique identifier for this request
        /// </summary>
        [JsonPropertyName("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// The ID of the capability to invoke
        /// </summary>
        [JsonPropertyName("capability_id")]
        public string CapabilityId { get; set; }

        /// <summary>
        /// The input parameters for the capability, conforming to its input schema
        /// </summary>
        [JsonPropertyName("input")]
        public Dictionary<string, object> Input { get; set; }

        /// <summary>
        /// Optional authentication information
        /// </summary>
        [JsonPropertyName("auth")]
        public Dictionary<string, object> Auth { get; set; }

        /// <summary>
        /// Optional additional metadata for the request
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }
    }

    /// <summary>
    /// Represents a response from an MCP capability invocation
    /// </summary>
    public class McpResponse
    {
        /// <summary>
        /// The unique identifier for the original request
        /// </summary>
        [JsonPropertyName("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// Indicates whether the request was successful
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// The output from the capability, conforming to its output schema
        /// </summary>
        [JsonPropertyName("output")]
        public Dictionary<string, object> Output { get; set; }

        /// <summary>
        /// Error information, if the request was not successful
        /// </summary>
        [JsonPropertyName("error")]
        public McpError Error { get; set; }

        /// <summary>
        /// Optional additional metadata for the response
        /// </summary>
        [JsonPropertyName("metadata")]
        public Dictionary<string, object> Metadata { get; set; }
    }

    /// <summary>
    /// Error information for a failed MCP request
    /// </summary>
    public class McpError
    {
        /// <summary>
        /// A machine-readable error code
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// A human-readable error message
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }

        /// <summary>
        /// Optional detailed error information
        /// </summary>
        [JsonPropertyName("details")]
        public Dictionary<string, object> Details { get; set; }
    }
} 