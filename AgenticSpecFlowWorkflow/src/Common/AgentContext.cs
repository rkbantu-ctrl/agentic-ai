namespace AgenticSpecFlowWorkflow.Common
{
    /// <summary>
    /// Represents the execution context for an agent
    /// </summary>
    public class AgentContext
    {
        /// <summary>
        /// Gets or sets the path to the OpenAPI contract file
        /// </summary>
        public string OpenApiContractPath { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the output directory for the agent's results
        /// </summary>
        public string OutputDirectory { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the input data from previous agents
        /// </summary>
        public Dictionary<string, object> Input { get; set; } = new Dictionary<string, object>();
        
        /// <summary>
        /// Gets or sets the configuration for the agent
        /// </summary>
        public Dictionary<string, object> Configuration { get; set; } = new Dictionary<string, object>();
    }
}