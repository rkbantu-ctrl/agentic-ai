using System;
using System.IO;
using System.Text.Json;

namespace SimpleAgenticWorkflow
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("=== Simple Agentic Workflow ===");
                
                // Check if OpenAPI file exists - accept from command line args if provided
                string openApiPath;
                if (args.Length > 0 && File.Exists(args[0]))
                {
                    openApiPath = args[0];
                    Console.WriteLine($"Using OpenAPI file from command line: {openApiPath}");
                }
                else
                {
                    var parentDir = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
                    openApiPath = Path.Combine(parentDir!, "AgenticSpecFlowWorkflow", "SampleOpenAPI.json");
                    Console.WriteLine($"Looking for default OpenAPI file at: {openApiPath}");
                }
                
                if (!File.Exists(openApiPath))
                {
                    Console.WriteLine("ERROR: OpenAPI file not found!");
                    return;
                }
                
                Console.WriteLine("OpenAPI file found, reading content...");
                string content = File.ReadAllText(openApiPath);
                Console.WriteLine($"OpenAPI file read, length: {content.Length} characters");
                
                // Parse JSON
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        AllowTrailingCommas = true,
                        PropertyNameCaseInsensitive = true
                    };
                    
                    var jsonDoc = JsonSerializer.Deserialize<JsonDocument>(content);
                    Console.WriteLine("Successfully parsed OpenAPI JSON!");
                    
                    // Create output directory
                    string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "SimpleWorkflowOutput");
                    Directory.CreateDirectory(outputDir);
                    Console.WriteLine($"Created output directory: {outputDir}");
                    
                    // Extract some basic info from the API spec to create a more relevant feature file
                    string apiTitle = "Unknown API";
                    string apiDescription = "API Description Not Available";
                    int endpointCount = 0;
                    
                    try {
                        var root = jsonDoc.RootElement;
                        
                        // Try to get info
                        if (root.TryGetProperty("info", out var infoElement)) {
                            if (infoElement.TryGetProperty("title", out var titleElement)) {
                                apiTitle = titleElement.GetString() ?? "Unknown API";
                            }
                            
                            if (infoElement.TryGetProperty("description", out var descElement)) {
                                apiDescription = descElement.GetString() ?? "API Description Not Available";
                            }
                        }
                        
                        // Try to count endpoints
                        if (root.TryGetProperty("paths", out var pathsElement)) {
                            endpointCount = pathsElement.EnumerateObject().Count();
                            Console.WriteLine($"Found {endpointCount} endpoints in the API spec");
                        }
                    }
                    catch (Exception ex) {
                        Console.WriteLine($"Error extracting API info: {ex.Message}");
                    }
                    
                    // Create a feature file based on the API spec
                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine($"Feature: {apiTitle} Testing");
                    sb.AppendLine($"  {apiDescription}");
                    sb.AppendLine("  As an API consumer");
                    sb.AppendLine("  I want to test the API endpoints");
                    sb.AppendLine("  So that I can ensure they work correctly");
                    sb.AppendLine();
                    
                    // Add a sample scenario
                    sb.AppendLine("Scenario: Verify API Health Check");
                    sb.AppendLine("  Given the API is available");
                    sb.AppendLine("  When I send a health check request");
                    sb.AppendLine("  Then I should receive a successful response");
                    sb.AppendLine();
                    
                    // Add a note about the endpoints
                    sb.AppendLine($"# This API has {endpointCount} endpoints that would be tested");
                    sb.AppendLine("# In a full implementation, each endpoint would have multiple test scenarios");
                    
                    string featureContent = sb.ToString();
                    string featurePath = Path.Combine(outputDir, "API_Tests.feature");
                    File.WriteAllText(featurePath, featureContent);
                    Console.WriteLine($"Created feature file: {featurePath}");
                    
                    // Create a more detailed report
                    string reportContent = $@"# {apiTitle} Workflow Report
Generated: {DateTime.Now}

## API Information
- Title: {apiTitle}
- Description: {apiDescription}
- Endpoints: {endpointCount}

## Processing Summary
- OpenAPI processed: Yes
- Feature files generated: 1
- Success: Yes

## Details
The workflow successfully processed the OpenAPI contract and would generate:
- Positive test scenarios for all endpoints
- Negative test scenarios for error handling
- Edge case testing for validation boundaries

## Next Steps
1. Generate detailed step definitions for all scenarios
2. Create integration tests with real API calls
3. Set up continuous testing pipeline
";
                    
                    string reportPath = Path.Combine(outputDir, "Report.md");
                    File.WriteAllText(reportPath, reportContent);
                    Console.WriteLine($"Created report file: {reportPath}");
                    
                    Console.WriteLine("\nWorkflow completed successfully!");
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"JSON parsing error: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}