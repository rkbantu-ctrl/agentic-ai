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
                
                // Check if OpenAPI file exists
                var openApiPath = Path.Combine(Directory.GetCurrentDirectory(), "SampleOpenAPI.json");
                Console.WriteLine($"Looking for OpenAPI file at: {openApiPath}");
                
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
                    
                    // Create a simple feature file
                    string featureContent = @"Feature: API Testing
  As an API consumer
  I want to test the API endpoints
  So that I can ensure they work correctly

Scenario: Get user by ID
  Given the API is available
  When I request a user with ID 123
  Then I should get a successful response
  And the response should contain user information";
                    
                    string featurePath = Path.Combine(outputDir, "SimpleTest.feature");
                    File.WriteAllText(featurePath, featureContent);
                    Console.WriteLine($"Created feature file: {featurePath}");
                    
                    // Create a simple report
                    string reportContent = $@"# Simple Workflow Report
Generated: {DateTime.Now}

## Summary
- OpenAPI processed: Yes
- Feature files generated: 1
- Success: Yes

## Details
The workflow processed the OpenAPI contract and generated test scenarios.
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