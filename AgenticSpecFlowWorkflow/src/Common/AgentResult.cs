using System.Text.Json.Serialization;

namespace AgenticSpecFlowWorkflow.Common
{
    /// <summary>
    /// Represents the result of an agent's execution
    /// </summary>
    public class AgentResult
    {
        /// <summary>
        /// Gets or sets whether the agent's execution was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Gets or sets the error message if the execution failed
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// Gets or sets the output data from the agent's execution
        /// </summary>
        public Dictionary<string, object>? Output { get; set; }
        
        /// <summary>
        /// Gets or sets the execution time in milliseconds
        /// </summary>
        public long ExecutionTimeMs { get; set; }

        [JsonIgnore]
        public Exception? Exception { get; set; }
    }
}