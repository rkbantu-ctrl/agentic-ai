namespace AgenticSpecFlowWorkflow.Common
{
    /// <summary>
    /// Interface for all agents in the workflow
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Gets the name of the agent
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Gets the description of the agent
        /// </summary>
        string Description { get; }
        
        /// <summary>
        /// Executes the agent's task
        /// </summary>
        /// <param name="context">The agent context containing input data and configuration</param>
        /// <returns>The result of the agent's execution</returns>
        Task<AgentResult> ExecuteAsync(AgentContext context);
    }
}