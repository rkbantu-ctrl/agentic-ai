using System;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using AgenticSpecFlowWorkflow.Common;

namespace AgenticSpecFlowWorkflow.Agents
{
    /// <summary>
    /// OpenApiGherkinScribe - An agent that analyzes OpenAPI contracts and generates Gherkin test scenarios
    /// </summary>
    public class OpenApiGherkinScribe : IAgent
    {
        private readonly ILogger<OpenApiGherkinScribe> _logger;
        private readonly OpenApiParser _openApiParser;

        public string Name => "OpenApiGherkinScribe";
        
        public string Description => "Agent that reads OpenAPI contracts and generates Gherkin test cases based on API specifications";

        public OpenApiGherkinScribe(ILogger<OpenApiGherkinScribe> logger, OpenApiParser openApiParser)
        {
            _logger = logger;
            _openApiParser = openApiParser;
        }

        public async Task<AgentResult> ExecuteAsync(AgentContext context)
        {
            _logger.LogInformation($"{Name} starting execution");
            
            try
            {
                // Step 1: Read the OpenAPI contract
                var openApiSpec = await _openApiParser.ParseAsync(context.OpenApiContractPath);
                _logger.LogInformation($"Successfully parsed OpenAPI contract: {context.OpenApiContractPath}");
                
                // Step 2: Analyze endpoints and operations
                var endpoints = _openApiParser.ExtractEndpoints(openApiSpec);
                _logger.LogInformation($"Found {endpoints.Count} endpoints in the API contract");
                
                // Step 3: Generate Gherkin scenarios for each endpoint
                var gherkinScenarios = new List<GherkinScenario>();
                
                foreach (var endpoint in endpoints)
                {
                    _logger.LogInformation($"Generating scenarios for endpoint: {endpoint.Path} ({endpoint.Method})");
                    
                    // Generate positive test scenarios
                    gherkinScenarios.Add(GeneratePositiveScenario(endpoint));
                    
                    // Generate negative test scenarios
                    gherkinScenarios.AddRange(GenerateNegativeScenarios(endpoint));
                    
                    // Generate edge case scenarios
                    gherkinScenarios.AddRange(GenerateEdgeCaseScenarios(endpoint));
                }
                
                _logger.LogInformation($"Generated {gherkinScenarios.Count} Gherkin scenarios");
                
                // Step 4: Save Gherkin scenarios to feature files
                var featureFilePath = Path.Combine(context.OutputDirectory, "Features");
                Directory.CreateDirectory(featureFilePath);
                
                SaveGherkinScenariosToFiles(gherkinScenarios, featureFilePath);
                _logger.LogInformation($"Saved Gherkin scenarios to feature files in: {featureFilePath}");
                
                // Return successful result with the output location
                return new AgentResult
                {
                    Success = true,
                    Output = new Dictionary<string, object>
                    {
                        { "FeatureFilePath", featureFilePath },
                        { "ScenarioCount", gherkinScenarios.Count },
                        { "Scenarios", gherkinScenarios }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {Name}: {ex.Message}");
                return new AgentResult
                {
                    Success = false,
                    ErrorMessage = $"Error generating Gherkin test cases: {ex.Message}"
                };
            }
        }

        private GherkinScenario GeneratePositiveScenario(ApiEndpoint endpoint)
        {
            // Create a positive test scenario for a successful API call
            var scenario = new GherkinScenario
            {
                ScenarioName = $"Successful {endpoint.OperationId} operation",
                Tags = new List<string> { "@positive", $"@{endpoint.Method.ToLower()}" },
                Steps = new List<string>()
            };

            // Add Given steps for required parameters and authentication
            scenario.Steps.Add($"Given I have a valid API client");
            
            if (endpoint.RequiresAuthentication)
            {
                scenario.Steps.Add($"And I am authenticated with valid credentials");
            }
            
            foreach (var param in endpoint.Parameters.Where(p => p.Required))
            {
                scenario.Steps.Add($"And I have a valid {param.Name} parameter");
            }
            
            // Add When step for the API call
            scenario.Steps.Add($"When I send a {endpoint.Method} request to \"{endpoint.Path}\"");
            
            // Add Then steps for expected responses
            scenario.Steps.Add($"Then the response status code should be {endpoint.SuccessStatusCode}");
            scenario.Steps.Add($"And the response should contain valid {endpoint.ResponseType} data");
            
            return scenario;
        }

        private List<GherkinScenario> GenerateNegativeScenarios(ApiEndpoint endpoint)
        {
            var scenarios = new List<GherkinScenario>();
            
            // Generate scenarios for missing required parameters
            foreach (var param in endpoint.Parameters.Where(p => p.Required))
            {
                var scenario = new GherkinScenario
                {
                    ScenarioName = $"Failed {endpoint.OperationId} operation with missing {param.Name}",
                    Tags = new List<string> { "@negative", $"@{endpoint.Method.ToLower()}" },
                    Steps = new List<string>()
                };
                
                scenario.Steps.Add($"Given I have a valid API client");
                
                if (endpoint.RequiresAuthentication)
                {
                    scenario.Steps.Add($"And I am authenticated with valid credentials");
                }
                
                foreach (var otherParam in endpoint.Parameters.Where(p => p.Required && p.Name != param.Name))
                {
                    scenario.Steps.Add($"And I have a valid {otherParam.Name} parameter");
                }
                
                scenario.Steps.Add($"But I do not provide the {param.Name} parameter");
                scenario.Steps.Add($"When I send a {endpoint.Method} request to \"{endpoint.Path}\"");
                scenario.Steps.Add($"Then the response status code should be 400");
                scenario.Steps.Add($"And the response should contain an error message about the missing {param.Name}");
                
                scenarios.Add(scenario);
            }
            
            // Add unauthorized access scenario if authentication is required
            if (endpoint.RequiresAuthentication)
            {
                var scenario = new GherkinScenario
                {
                    ScenarioName = $"Failed {endpoint.OperationId} operation with unauthorized access",
                    Tags = new List<string> { "@negative", "@security", $"@{endpoint.Method.ToLower()}" },
                    Steps = new List<string>()
                };
                
                scenario.Steps.Add($"Given I have a valid API client");
                scenario.Steps.Add($"But I am not authenticated");
                scenario.Steps.Add($"When I send a {endpoint.Method} request to \"{endpoint.Path}\"");
                scenario.Steps.Add($"Then the response status code should be 401");
                scenario.Steps.Add($"And the response should contain an authentication error message");
                
                scenarios.Add(scenario);
            }
            
            return scenarios;
        }

        private List<GherkinScenario> GenerateEdgeCaseScenarios(ApiEndpoint endpoint)
        {
            var scenarios = new List<GherkinScenario>();
            
            // Generate scenarios for data validation edge cases
            foreach (var param in endpoint.Parameters.Where(p => p.Type == "string" || p.Type == "integer" || p.Type == "number"))
            {
                if (param.Type == "string")
                {
                    var scenario = new GherkinScenario
                    {
                        ScenarioName = $"Validate {endpoint.OperationId} with extremely long {param.Name}",
                        Tags = new List<string> { "@edge-case", $"@{endpoint.Method.ToLower()}" },
                        Steps = new List<string>()
                    };
                    
                    scenario.Steps.Add($"Given I have a valid API client");
                    scenario.Steps.Add($"And I have a {param.Name} parameter with 10000 characters");
                    scenario.Steps.Add($"When I send a {endpoint.Method} request to \"{endpoint.Path}\"");
                    scenario.Steps.Add($"Then the API should handle the request appropriately");
                    
                    scenarios.Add(scenario);
                }
                else if (param.Type == "integer" || param.Type == "number")
                {
                    var scenario = new GherkinScenario
                    {
                        ScenarioName = $"Validate {endpoint.OperationId} with boundary value for {param.Name}",
                        Tags = new List<string> { "@edge-case", $"@{endpoint.Method.ToLower()}" },
                        Steps = new List<string>()
                    };
                    
                    scenario.Steps.Add($"Given I have a valid API client");
                    scenario.Steps.Add($"And I have a {param.Name} parameter at the maximum allowed value");
                    scenario.Steps.Add($"When I send a {endpoint.Method} request to \"{endpoint.Path}\"");
                    scenario.Steps.Add($"Then the API should handle the request appropriately");
                    
                    scenarios.Add(scenario);
                }
            }
            
            return scenarios;
        }

        private void SaveGherkinScenariosToFiles(List<GherkinScenario> scenarios, string outputDirectory)
        {
            // Group scenarios by endpoint/functionality
            var groupedScenarios = scenarios
                .GroupBy(s => s.Tags.FirstOrDefault(t => t.StartsWith("@")) ?? "@api")
                .ToDictionary(g => g.Key, g => g.ToList());
            
            foreach (var group in groupedScenarios)
            {
                var featureName = group.Key.Replace("@", "");
                var featureFile = Path.Combine(outputDirectory, $"{featureName}.feature");
                
                var featureContent = new StringBuilder();
                featureContent.AppendLine($"Feature: {char.ToUpper(featureName[0]) + featureName.Substring(1)} API Tests");
                featureContent.AppendLine($"  As an API user");
                featureContent.AppendLine($"  I want to ensure the {featureName} endpoints work correctly");
                featureContent.AppendLine($"  So that I can rely on the API functionality");
                featureContent.AppendLine();
                
                foreach (var scenario in group.Value)
                {
                    featureContent.AppendLine($"  {string.Join(" ", scenario.Tags)}");
                    featureContent.AppendLine($"  Scenario: {scenario.ScenarioName}");
                    
                    foreach (var step in scenario.Steps)
                    {
                        featureContent.AppendLine($"    {step}");
                    }
                    
                    featureContent.AppendLine();
                }
                
                File.WriteAllText(featureFile, featureContent.ToString());
            }
        }
    }
}