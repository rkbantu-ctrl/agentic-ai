namespace AgenticSpecFlowWorkflow.Common
{
    /// <summary>
    /// Represents a Gherkin test scenario
    /// </summary>
    public class GherkinScenario
    {
        /// <summary>
        /// Gets or sets the name of the scenario
        /// </summary>
        public string ScenarioName { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the tags of the scenario
        /// </summary>
        public List<string> Tags { get; set; } = new List<string>();
        
        /// <summary>
        /// Gets or sets the steps of the scenario
        /// </summary>
        public List<string> Steps { get; set; } = new List<string>();
    }
}