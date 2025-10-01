using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AgenticSpecFlowWorkflow.Agents;
using AgenticSpecFlowWorkflow.Common;
using AgenticSpecFlowWorkflow.Workflow;
using System.Text;

namespace AgenticSpecFlowWorkflow
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Force console output to ensure we see it
            Console.WriteLine("=== AGENTIC SPECFLOW WORKFLOW STARTING ===");
            
            // Add this to ensure we see console output
            Console.OutputEncoding = Encoding.UTF8;
            
            // This will log directly to the console regardless of logging configuration
            Console.WriteLine("Running AgenticSpecFlowWorkflow application...");
            
            try 
            {
                var host = CreateHostBuilder(args).Build();
                Console.WriteLine("Host builder created successfully");
                
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    Console.WriteLine("Logger service initialized");
                
                    try
                    {
                        logger.LogInformation("Starting AgenticSpecFlowWorkflow");
                        Console.WriteLine("Starting AgenticSpecFlowWorkflow - Check console output");
                        
                        // Get the orchestrator
                        var orchestrator = services.GetRequiredService<WorkflowOrchestrator>();
                        logger.LogInformation("Orchestrator initialized");
                        Console.WriteLine("Orchestrator initialized");
                        
                        // Create workflow configuration
                        var config = GetWorkflowConfig(args);
                        logger.LogInformation($"Configuration created, output directory: {config.OutputDirectory}");
                        Console.WriteLine($"Configuration created, output directory: {config.OutputDirectory}");
                        
                        // Check if OpenAPI file exists
                        if (!File.Exists(config.OpenApiContractPath))
                        {
                            logger.LogError($"OpenAPI file not found: {config.OpenApiContractPath}");
                            Console.WriteLine($"ERROR: OpenAPI file not found: {config.OpenApiContractPath}");
                            return;
                        }
                        
                        Console.WriteLine($"OpenAPI file found: {config.OpenApiContractPath}");
                        logger.LogInformation($"OpenAPI file found: {config.OpenApiContractPath}");
                        
                        // Create output directory if it doesn't exist
                        Directory.CreateDirectory(config.OutputDirectory);
                        logger.LogInformation($"Created output directory: {config.OutputDirectory}");
                        Console.WriteLine($"Created output directory: {config.OutputDirectory}");
                        
                        // Execute the workflow
                        logger.LogInformation($"Executing workflow with OpenAPI contract: {config.OpenApiContractPath}");
                        Console.WriteLine($"Executing workflow with OpenAPI contract: {config.OpenApiContractPath}");
                        
                        var result = await orchestrator.ExecuteWorkflowAsync(config);
                        
                        // Display result
                        if (result.Success)
                        {
                            logger.LogInformation($"Workflow completed successfully in {result.ExecutionTimeMs / 1000.0:F2} seconds");
                            logger.LogInformation($"Output directory: {config.OutputDirectory}");
                            Console.WriteLine($"Workflow completed successfully in {result.ExecutionTimeMs / 1000.0:F2} seconds");
                            Console.WriteLine($"Output directory: {config.OutputDirectory}");
                        }
                        else
                        {
                            logger.LogError($"Workflow failed: {result.ErrorMessage}");
                            Console.WriteLine($"ERROR: Workflow failed: {result.ErrorMessage}");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred while executing the workflow");
                        Console.WriteLine($"ERROR: An error occurred while executing the workflow: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL ERROR: Failed to initialize application: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    // Register common services
                    services.AddSingleton<OpenApiParser>();
                    services.AddSingleton<TestCodeFactory>();
                    services.AddSingleton<TestRunner>();
                    services.AddSingleton<ReportGenerator>();
                    
                    // Register agents
                    services.AddSingleton<OpenApiGherkinScribe>();
                    services.AddSingleton<TestCodeGenerator>();
                    services.AddSingleton<TestExecutor>();
                    
                    // Register orchestrator
                    services.AddSingleton<WorkflowOrchestrator>();
                })
                .ConfigureLogging((hostContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                });
        
        private static WorkflowConfig GetWorkflowConfig(string[] args)
        {
            // In a real application, you would parse command-line arguments and/or read from a configuration file
            // For this demo, we'll use hardcoded values
            
            // Create base output directory
            var outputDir = Path.Combine(Directory.GetCurrentDirectory(), "WorkflowOutput", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            
            return new WorkflowConfig
            {
                // Use a sample OpenAPI contract path or check command-line args
                OpenApiContractPath = args.Length > 0 ? args[0] : Path.Combine(Directory.GetCurrentDirectory(), "SampleOpenAPI.json"),
                OutputDirectory = outputDir,
                AgentConfigurations = new Dictionary<string, Dictionary<string, object>>
                {
                    {
                        "OpenApiGherkinScribe",
                        new Dictionary<string, object>
                        {
                            { "GeneratePositiveScenarios", true },
                            { "GenerateNegativeScenarios", true },
                            { "GenerateEdgeCaseScenarios", true }
                        }
                    },
                    {
                        "TestCodeGenerator",
                        new Dictionary<string, object>
                        {
                            { "GenerateAcceptanceTests", true },
                            { "GenerateIntegrationTests", true },
                            { "GenerateLiveTests", true }
                        }
                    },
                    {
                        "TestExecutor",
                        new Dictionary<string, object>
                        {
                            { "RunAcceptanceTests", true },
                            { "RunIntegrationTests", true },
                            { "RunLiveTests", true },
                            { "GenerateReports", true }
                        }
                    }
                }
            };
        }
    }
}