using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AgenticSpecFlowWorkflow.Agents;
using AgenticSpecFlowWorkflow.Common;

namespace AgenticSpecFlowWorkflow.Workflow
{
    /// <summary>
    /// Orchestrator for the agentic workflow
    /// </summary>
    public class WorkflowOrchestrator
    {
        private readonly ILogger<WorkflowOrchestrator> _logger;
        private readonly OpenApiGherkinScribe _openApiGherkinScribe;
        private readonly TestCodeGenerator _testCodeGenerator;
        private readonly TestExecutor _testExecutor;
        
        public WorkflowOrchestrator(
            ILogger<WorkflowOrchestrator> logger,
            OpenApiGherkinScribe openApiGherkinScribe,
            TestCodeGenerator testCodeGenerator,
            TestExecutor testExecutor)
        {
            _logger = logger;
            _openApiGherkinScribe = openApiGherkinScribe;
            _testCodeGenerator = testCodeGenerator;
            _testExecutor = testExecutor;
        }
        
        /// <summary>
        /// Executes the workflow with the specified configuration
        /// </summary>
        public async Task<WorkflowResult> ExecuteWorkflowAsync(WorkflowConfig config)
        {
            _logger.LogInformation("Starting workflow execution");
            var workflowResult = new WorkflowResult();
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // Create base output directory
                Directory.CreateDirectory(config.OutputDirectory);
                _logger.LogInformation($"Created output directory: {config.OutputDirectory}");
                
                // Step 1: Execute OpenApiGherkinScribe agent
                _logger.LogInformation($"Executing agent: {_openApiGherkinScribe.Name}");
                var agentAContext = new AgentContext
                {
                    OpenApiContractPath = config.OpenApiContractPath,
                    OutputDirectory = Path.Combine(config.OutputDirectory, "Stage1_GherkinScenarios"),
                    Configuration = config.AgentConfigurations.GetValueOrDefault("OpenApiGherkinScribe", new Dictionary<string, object>())
                };
                
                Directory.CreateDirectory(agentAContext.OutputDirectory);
                var agentAResult = await _openApiGherkinScribe.ExecuteAsync(agentAContext);
                workflowResult.AgentResults.Add(_openApiGherkinScribe.Name, agentAResult);
                
                if (!agentAResult.Success)
                {
                    _logger.LogError($"Agent {_openApiGherkinScribe.Name} failed: {agentAResult.ErrorMessage}");
                    workflowResult.Success = false;
                    workflowResult.ErrorMessage = $"Workflow failed at agent {_openApiGherkinScribe.Name}: {agentAResult.ErrorMessage}";
                    return workflowResult;
                }
                
                // Step 2: Execute TestCodeGenerator agent
                _logger.LogInformation($"Executing agent: {_testCodeGenerator.Name}");
                var agentBContext = new AgentContext
                {
                    OpenApiContractPath = config.OpenApiContractPath,
                    OutputDirectory = Path.Combine(config.OutputDirectory, "Stage2_TestImplementations"),
                    Input = agentAResult.Output!,
                    Configuration = config.AgentConfigurations.GetValueOrDefault("TestCodeGenerator", new Dictionary<string, object>())
                };
                
                Directory.CreateDirectory(agentBContext.OutputDirectory);
                var agentBResult = await _testCodeGenerator.ExecuteAsync(agentBContext);
                workflowResult.AgentResults.Add(_testCodeGenerator.Name, agentBResult);
                
                if (!agentBResult.Success)
                {
                    _logger.LogError($"Agent {_testCodeGenerator.Name} failed: {agentBResult.ErrorMessage}");
                    workflowResult.Success = false;
                    workflowResult.ErrorMessage = $"Workflow failed at agent {_testCodeGenerator.Name}: {agentBResult.ErrorMessage}";
                    return workflowResult;
                }
                
                // Step 3: Execute TestExecutor agent
                _logger.LogInformation($"Executing agent: {_testExecutor.Name}");
                var agentCContext = new AgentContext
                {
                    OpenApiContractPath = config.OpenApiContractPath,
                    OutputDirectory = Path.Combine(config.OutputDirectory, "Stage3_TestReports"),
                    Input = agentBResult.Output!,
                    Configuration = config.AgentConfigurations.GetValueOrDefault("TestExecutor", new Dictionary<string, object>())
                };
                
                Directory.CreateDirectory(agentCContext.OutputDirectory);
                var agentCResult = await _testExecutor.ExecuteAsync(agentCContext);
                workflowResult.AgentResults.Add(_testExecutor.Name, agentCResult);
                
                if (!agentCResult.Success)
                {
                    _logger.LogError($"Agent {_testExecutor.Name} failed: {agentCResult.ErrorMessage}");
                    workflowResult.Success = false;
                    workflowResult.ErrorMessage = $"Workflow failed at agent {_testExecutor.Name}: {agentCResult.ErrorMessage}";
                    return workflowResult;
                }
                
                // Set workflow result
                workflowResult.Success = true;
                workflowResult.Output = new Dictionary<string, object>
                {
                    { "OpenApiContractPath", config.OpenApiContractPath },
                    { "OutputDirectory", config.OutputDirectory },
                    { "GherkinScenariosPath", agentAContext.OutputDirectory },
                    { "TestImplementationsPath", agentBContext.OutputDirectory },
                    { "TestReportsPath", agentCContext.OutputDirectory },
                    { "TestResultsSummary", agentCResult.Output!["TestResultsSummary"] }
                };
                
                _logger.LogInformation("Workflow execution completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Workflow execution failed");
                workflowResult.Success = false;
                workflowResult.ErrorMessage = $"Workflow execution failed: {ex.Message}";
            }
            finally
            {
                stopwatch.Stop();
                workflowResult.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }
            
            return workflowResult;
        }
    }
    
    /// <summary>
    /// Configuration for the workflow
    /// </summary>
    public class WorkflowConfig
    {
        /// <summary>
        /// Gets or sets the path to the OpenAPI contract file
        /// </summary>
        public string OpenApiContractPath { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the output directory for the workflow results
        /// </summary>
        public string OutputDirectory { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the configurations for individual agents
        /// </summary>
        public Dictionary<string, Dictionary<string, object>> AgentConfigurations { get; set; } = 
            new Dictionary<string, Dictionary<string, object>>();
    }
    
    /// <summary>
    /// Result of the workflow execution
    /// </summary>
    public class WorkflowResult
    {
        /// <summary>
        /// Gets or sets whether the workflow execution was successful
        /// </summary>
        public bool Success { get; set; }
        
        /// <summary>
        /// Gets or sets the error message if the workflow execution failed
        /// </summary>
        public string? ErrorMessage { get; set; }
        
        /// <summary>
        /// Gets or sets the output of the workflow execution
        /// </summary>
        public Dictionary<string, object>? Output { get; set; }
        
        /// <summary>
        /// Gets or sets the execution time of the workflow in milliseconds
        /// </summary>
        public long ExecutionTimeMs { get; set; }
        
        /// <summary>
        /// Gets or sets the results of individual agents
        /// </summary>
        public Dictionary<string, AgentResult> AgentResults { get; set; } = new Dictionary<string, AgentResult>();
    }
}