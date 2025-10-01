using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AgenticSpecFlowWorkflow.Common;

namespace AgenticSpecFlowWorkflow.Agents
{
    /// <summary>
    /// TestCodeGenerator - An agent that generates implementation code for acceptance tests and integration tests
    /// </summary>
    public class TestCodeGenerator : IAgent
    {
        private readonly ILogger<TestCodeGenerator> _logger;
        private readonly TestCodeFactory _testCodeFactory;

        public string Name => "TestCodeGenerator";
        
        public string Description => "Agent that reviews Gherkin test cases and generates implementation code for acceptance tests, integration tests, and live tests";

        public TestCodeGenerator(ILogger<TestCodeGenerator> logger, TestCodeFactory testCodeFactory)
        {
            _logger = logger;
            _testCodeFactory = testCodeFactory;
        }

        public async Task<AgentResult> ExecuteAsync(AgentContext context)
        {
            _logger.LogInformation($"{Name} starting execution");
            
            try
            {
                // Step 1: Read the Gherkin scenarios created by the previous agent
                if (!context.Input.ContainsKey("FeatureFilePath") || 
                    !context.Input.ContainsKey("Scenarios"))
                {
                    throw new InvalidOperationException("Required input from previous agent not found");
                }
                
                var featureFilePath = context.Input["FeatureFilePath"] as string;
                var scenarios = context.Input["Scenarios"] as List<GherkinScenario>;
                
                _logger.LogInformation($"Found {scenarios?.Count ?? 0} scenarios to implement");
                
                // Step 2: Create test implementation directory structure
                var testImplPath = Path.Combine(context.OutputDirectory, "TestImplementations");
                var acceptanceTestsPath = Path.Combine(testImplPath, "AcceptanceTests");
                var integrationTestsPath = Path.Combine(testImplPath, "IntegrationTests");
                var liveTestsPath = Path.Combine(testImplPath, "LiveTests");
                
                Directory.CreateDirectory(acceptanceTestsPath);
                Directory.CreateDirectory(integrationTestsPath);
                Directory.CreateDirectory(liveTestsPath);
                
                _logger.LogInformation($"Created test implementation directory structure at: {testImplPath}");
                
                // Step 3: Generate the necessary code files
                var generatedFiles = new List<string>();
                
                // Generate test step definitions
                generatedFiles.AddRange(await GenerateStepDefinitions(scenarios, acceptanceTestsPath));
                _logger.LogInformation($"Generated step definitions for acceptance tests");
                
                // Generate API client for integration tests
                generatedFiles.AddRange(await GenerateApiClient(context.OpenApiContractPath, integrationTestsPath));
                _logger.LogInformation($"Generated API client for integration tests");
                
                // Generate integration test implementations
                generatedFiles.AddRange(await GenerateIntegrationTests(scenarios, integrationTestsPath));
                _logger.LogInformation($"Generated integration test implementations");
                
                // Generate live test implementations
                generatedFiles.AddRange(await GenerateLiveTests(scenarios, liveTestsPath));
                _logger.LogInformation($"Generated live test implementations");
                
                // Step 4: Generate test configuration files
                generatedFiles.AddRange(await GenerateTestConfigurations(testImplPath));
                _logger.LogInformation($"Generated test configuration files");
                
                // Return successful result with the output locations
                return new AgentResult
                {
                    Success = true,
                    Output = new Dictionary<string, object>
                    {
                        { "TestImplementationsPath", testImplPath },
                        { "AcceptanceTestsPath", acceptanceTestsPath },
                        { "IntegrationTestsPath", integrationTestsPath },
                        { "LiveTestsPath", liveTestsPath },
                        { "GeneratedFiles", generatedFiles }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {Name}: {ex.Message}");
                return new AgentResult
                {
                    Success = false,
                    ErrorMessage = $"Error generating test implementations: {ex.Message}"
                };
            }
        }

        private async Task<List<string>> GenerateStepDefinitions(List<GherkinScenario> scenarios, string outputPath)
        {
            var generatedFiles = new List<string>();
            
            // Group the steps by their type (Given, When, Then)
            var allSteps = scenarios.SelectMany(s => s.Steps).Distinct().ToList();
            var givenSteps = allSteps.Where(s => s.StartsWith("Given") || s.StartsWith("And I have") || s.StartsWith("And I am")).ToList();
            var whenSteps = allSteps.Where(s => s.StartsWith("When") || s.StartsWith("And I send")).ToList();
            var thenSteps = allSteps.Where(s => s.StartsWith("Then") || s.StartsWith("And the response")).ToList();
            
            // Generate Given step definitions
            var givenStepDefFile = Path.Combine(outputPath, "GivenSteps.cs");
            var givenStepContent = _testCodeFactory.GenerateStepDefinitionClass("GivenSteps", givenSteps);
            await File.WriteAllTextAsync(givenStepDefFile, givenStepContent);
            generatedFiles.Add(givenStepDefFile);
            
            // Generate When step definitions
            var whenStepDefFile = Path.Combine(outputPath, "WhenSteps.cs");
            var whenStepContent = _testCodeFactory.GenerateStepDefinitionClass("WhenSteps", whenSteps);
            await File.WriteAllTextAsync(whenStepDefFile, whenStepContent);
            generatedFiles.Add(whenStepDefFile);
            
            // Generate Then step definitions
            var thenStepDefFile = Path.Combine(outputPath, "ThenSteps.cs");
            var thenStepContent = _testCodeFactory.GenerateStepDefinitionClass("ThenSteps", thenSteps);
            await File.WriteAllTextAsync(thenStepDefFile, thenStepContent);
            generatedFiles.Add(thenStepDefFile);
            
            // Generate test context to share data between steps
            var contextFile = Path.Combine(outputPath, "TestContext.cs");
            var contextContent = _testCodeFactory.GenerateTestContextClass();
            await File.WriteAllTextAsync(contextFile, contextContent);
            generatedFiles.Add(contextFile);
            
            return generatedFiles;
        }

        private async Task<List<string>> GenerateApiClient(string openApiContractPath, string outputPath)
        {
            var generatedFiles = new List<string>();
            
            // Generate base API client
            var apiClientFile = Path.Combine(outputPath, "ApiClient.cs");
            var apiClientContent = _testCodeFactory.GenerateApiClientClass(openApiContractPath);
            await File.WriteAllTextAsync(apiClientFile, apiClientContent);
            generatedFiles.Add(apiClientFile);
            
            // Generate request/response models based on OpenAPI schema
            var modelsDir = Path.Combine(outputPath, "Models");
            Directory.CreateDirectory(modelsDir);
            
            var modelFiles = _testCodeFactory.GenerateModelClasses(openApiContractPath, modelsDir);
            generatedFiles.AddRange(modelFiles);
            
            return generatedFiles;
        }

        private async Task<List<string>> GenerateIntegrationTests(List<GherkinScenario> scenarios, string outputPath)
        {
            var generatedFiles = new List<string>();
            
            // Group scenarios by endpoint/feature
            var scenarioGroups = scenarios
                .GroupBy(s => s.Tags.FirstOrDefault(t => t.StartsWith("@") && !t.StartsWith("@positive") && !t.StartsWith("@negative") && !t.StartsWith("@edge-case")) ?? "@api")
                .ToDictionary(g => g.Key, g => g.ToList());
            
            foreach (var group in scenarioGroups)
            {
                var featureName = group.Key.Replace("@", "");
                var testClassName = $"{char.ToUpper(featureName[0]) + featureName.Substring(1)}IntegrationTests";
                var testFile = Path.Combine(outputPath, $"{testClassName}.cs");
                
                var testContent = _testCodeFactory.GenerateIntegrationTestClass(testClassName, group.Value);
                await File.WriteAllTextAsync(testFile, testContent);
                generatedFiles.Add(testFile);
            }
            
            return generatedFiles;
        }

        private async Task<List<string>> GenerateLiveTests(List<GherkinScenario> scenarios, string outputPath)
        {
            var generatedFiles = new List<string>();
            
            // Group scenarios by endpoint/feature
            var scenarioGroups = scenarios
                .GroupBy(s => s.Tags.FirstOrDefault(t => t.StartsWith("@") && !t.StartsWith("@positive") && !t.StartsWith("@negative") && !t.StartsWith("@edge-case")) ?? "@api")
                .ToDictionary(g => g.Key, g => g.ToList());
            
            foreach (var group in scenarioGroups)
            {
                var featureName = group.Key.Replace("@", "");
                var testClassName = $"{char.ToUpper(featureName[0]) + featureName.Substring(1)}LiveTests";
                var testFile = Path.Combine(outputPath, $"{testClassName}.cs");
                
                var testContent = _testCodeFactory.GenerateLiveTestClass(testClassName, group.Value);
                await File.WriteAllTextAsync(testFile, testContent);
                generatedFiles.Add(testFile);
            }
            
            return generatedFiles;
        }

        private async Task<List<string>> GenerateTestConfigurations(string basePath)
        {
            var generatedFiles = new List<string>();
            
            // Generate test configuration file
            var configFile = Path.Combine(basePath, "testsettings.json");
            var configContent = _testCodeFactory.GenerateTestConfigurationFile();
            await File.WriteAllTextAsync(configFile, configContent);
            generatedFiles.Add(configFile);
            
            // Generate specflow.json configuration
            var specflowConfigFile = Path.Combine(basePath, "specflow.json");
            var specflowConfigContent = _testCodeFactory.GenerateSpecFlowConfigFile();
            await File.WriteAllTextAsync(specflowConfigFile, specflowConfigContent);
            generatedFiles.Add(specflowConfigFile);
            
            return generatedFiles;
        }
    }
}