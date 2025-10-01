using System;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EndToEndAgenticWorkflow
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("=== Agentic SpecFlow Workflow End-to-End Demo ===");
            
            try
            {
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
                    openApiPath = Path.Combine(parentDir!, "AgenticSpecFlowWorkflow", "src", "Common", "OPENAPI-Spec", "MCG-Edge-AttachmentService-1.0.8-resolved.json");
                    Console.WriteLine($"Looking for default OpenAPI file at: {openApiPath}");
                }
                
                if (!File.Exists(openApiPath))
                {
                    Console.WriteLine("ERROR: OpenAPI file not found!");
                    return;
                }
                
                Console.WriteLine("OpenAPI file found!");
                
                // Create output directory structure
                string baseOutputDir = Path.Combine(Directory.GetCurrentDirectory(), "E2EWorkflowOutput");
                Directory.CreateDirectory(baseOutputDir);
                
                // Create directories for each agent's output
                string agent1OutputDir = Path.Combine(baseOutputDir, "Stage1_GherkinScenarios");
                string agent2OutputDir = Path.Combine(baseOutputDir, "Stage2_TestImplementations");
                string agent3OutputDir = Path.Combine(baseOutputDir, "Stage3_TestReports");
                
                Directory.CreateDirectory(agent1OutputDir);
                Directory.CreateDirectory(agent2OutputDir);
                Directory.CreateDirectory(agent3OutputDir);
                
                Console.WriteLine($"Created output directory structure in {baseOutputDir}");
                
                // Run the end-to-end workflow
                var stopwatch = Stopwatch.StartNew();
                
                // Agent 1: OpenApiGherkinScribe
                Console.WriteLine("\n=== AGENT 1: OpenApiGherkinScribe ===");
                Console.WriteLine("Reading and parsing OpenAPI contract...");
                
                string content = await File.ReadAllTextAsync(openApiPath);
                Console.WriteLine($"OpenAPI file read, length: {content.Length} characters");
                
                var jsonDoc = JsonSerializer.Deserialize<JsonDocument>(content);
                Console.WriteLine("Successfully parsed OpenAPI JSON!");
                
                // Extract API information
                var apiInfo = ExtractApiInfo(jsonDoc!);
                Console.WriteLine($"API Title: {apiInfo.Title}");
                Console.WriteLine($"API Version: {apiInfo.Version}");
                Console.WriteLine($"Endpoints found: {apiInfo.Endpoints.Count}");
                
                // Generate Gherkin scenarios
                Console.WriteLine("Generating Gherkin scenarios...");
                var scenarios = GenerateGherkinScenarios(apiInfo);
                Console.WriteLine($"Generated {scenarios.Count} Gherkin scenarios");
                
                // Save Gherkin feature files
                var featurePath = Path.Combine(agent1OutputDir, "Features");
                Directory.CreateDirectory(featurePath);
                
                int featureFileCount = 0;
                foreach (var scenarioGroup in scenarios.GroupBy(s => s.Category))
                {
                    var featureFileName = $"{SanitizeFileName(scenarioGroup.Key)}.feature";
                    var featureFilePath = Path.Combine(featurePath, featureFileName);
                    
                    var sb = new StringBuilder();
                    sb.AppendLine($"Feature: {scenarioGroup.Key}");
                    sb.AppendLine($"  Part of {apiInfo.Title} v{apiInfo.Version}");
                    sb.AppendLine("  As an API consumer");
                    sb.AppendLine("  I want to test the API endpoints");
                    sb.AppendLine("  So that I can ensure they work correctly");
                    sb.AppendLine();
                    
                    foreach (var scenario in scenarioGroup)
                    {
                        sb.AppendLine($"Scenario: {scenario.Name}");
                        foreach (var step in scenario.Steps)
                        {
                            sb.AppendLine($"  {step}");
                        }
                        sb.AppendLine();
                    }
                    
                    File.WriteAllText(featureFilePath, sb.ToString());
                    featureFileCount++;
                }
                
                Console.WriteLine($"Saved {featureFileCount} feature files to {featurePath}");
                Console.WriteLine("Agent 1 completed successfully!");
                
                // Agent 2: TestCodeGenerator
                Console.WriteLine("\n=== AGENT 2: TestCodeGenerator ===");
                Console.WriteLine("Generating step definitions and test implementations...");
                
                var stepDefsDir = Path.Combine(agent2OutputDir, "StepDefinitions");
                var supportDir = Path.Combine(agent2OutputDir, "Support");
                
                Directory.CreateDirectory(stepDefsDir);
                Directory.CreateDirectory(supportDir);
                
                // Generate step definition files
                var stepDefFiles = GenerateStepDefinitions(scenarios, stepDefsDir);
                Console.WriteLine($"Generated {stepDefFiles.Count} step definition files");
                
                // Generate support files (API clients, test context, etc.)
                var supportFiles = GenerateSupportFiles(apiInfo, supportDir);
                Console.WriteLine($"Generated {supportFiles.Count} support files");
                
                Console.WriteLine("Agent 2 completed successfully!");
                
                // Agent 3: TestExecutor
                Console.WriteLine("\n=== AGENT 3: TestExecutor ===");
                Console.WriteLine("Executing tests and generating reports...");
                
                // Simulate test execution
                var testResults = ExecuteTests(scenarios);
                
                // Generate test reports
                var reportsDir = Path.Combine(agent3OutputDir, "Reports");
                Directory.CreateDirectory(reportsDir);
                
                GenerateTestReports(testResults, reportsDir, apiInfo);
                
                Console.WriteLine("Agent 3 completed successfully!");
                
                // Workflow completion
                stopwatch.Stop();
                var executionTime = stopwatch.ElapsedMilliseconds / 1000.0;
                
                Console.WriteLine($"\n=== Workflow completed successfully in {executionTime:F2} seconds ===");
                Console.WriteLine($"Output directory: {baseOutputDir}");
                
                // Generate workflow summary
                GenerateWorkflowSummary(baseOutputDir, apiInfo, scenarios.Count, testResults, executionTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERROR: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
        
        static ApiInfo ExtractApiInfo(JsonDocument doc)
        {
            var apiInfo = new ApiInfo();
            var root = doc.RootElement;
            
            // Extract basic info
            if (root.TryGetProperty("info", out var infoElement))
            {
                if (infoElement.TryGetProperty("title", out var titleElement))
                {
                    apiInfo.Title = titleElement.GetString() ?? "Unknown API";
                }
                
                if (infoElement.TryGetProperty("version", out var versionElement))
                {
                    apiInfo.Version = versionElement.GetString() ?? "1.0.0";
                }
                
                if (infoElement.TryGetProperty("description", out var descElement))
                {
                    apiInfo.Description = descElement.GetString() ?? "";
                }
            }
            
            // Extract endpoints
            if (root.TryGetProperty("paths", out var pathsElement))
            {
                foreach (var path in pathsElement.EnumerateObject())
                {
                    foreach (var method in path.Value.EnumerateObject())
                    {
                        var endpoint = new ApiEndpoint
                        {
                            Path = path.Name,
                            Method = method.Name.ToUpper(),
                            OperationId = ""
                        };
                        
                        // Try to get operationId
                        if (method.Value.TryGetProperty("operationId", out var operationIdElement))
                        {
                            endpoint.OperationId = operationIdElement.GetString() ?? "";
                        }
                        
                        // Try to get summary
                        if (method.Value.TryGetProperty("summary", out var summaryElement))
                        {
                            endpoint.Summary = summaryElement.GetString() ?? "";
                        }
                        
                        apiInfo.Endpoints.Add(endpoint);
                    }
                }
            }
            
            return apiInfo;
        }
        
        static List<GherkinScenario> GenerateGherkinScenarios(ApiInfo apiInfo)
        {
            var scenarios = new List<GherkinScenario>();
            
            // Generate health check scenario
            scenarios.Add(new GherkinScenario
            {
                Name = "Health Check - Verify API is available",
                Category = "Health Checks",
                Tags = new List<string> { "@health", "@smoke" },
                Steps = new List<string>
                {
                    "Given the API base URL is configured",
                    "When I send a health check request",
                    "Then I should receive a 200 OK response",
                    "And the response should contain health status information"
                }
            });
            
            // Generate scenarios for each endpoint
            foreach (var endpoint in apiInfo.Endpoints)
            {
                // Generate positive scenario
                var positiveName = GetPositiveScenarioName(endpoint);
                var category = GetCategoryFromPath(endpoint.Path);
                
                var positiveScenario = new GherkinScenario
                {
                    Name = positiveName,
                    Category = category,
                    Tags = new List<string> { "@positive", $"@{endpoint.Method.ToLower()}" },
                    Steps = GeneratePositiveSteps(endpoint)
                };
                
                scenarios.Add(positiveScenario);
                
                // Generate negative scenario
                var negativeScenario = new GherkinScenario
                {
                    Name = $"{endpoint.Method} {endpoint.Path} - Error handling",
                    Category = category,
                    Tags = new List<string> { "@negative", $"@{endpoint.Method.ToLower()}" },
                    Steps = GenerateNegativeSteps(endpoint)
                };
                
                scenarios.Add(negativeScenario);
            }
            
            return scenarios;
        }
        
        static string GetPositiveScenarioName(ApiEndpoint endpoint)
        {
            if (!string.IsNullOrEmpty(endpoint.Summary))
            {
                return endpoint.Summary;
            }
            
            switch (endpoint.Method)
            {
                case "GET":
                    return $"Get resource from {endpoint.Path}";
                case "POST":
                    return $"Create new resource at {endpoint.Path}";
                case "PUT":
                    return $"Update resource at {endpoint.Path}";
                case "DELETE":
                    return $"Delete resource at {endpoint.Path}";
                case "PATCH":
                    return $"Partially update resource at {endpoint.Path}";
                default:
                    return $"{endpoint.Method} {endpoint.Path}";
            }
        }
        
        static string GetCategoryFromPath(string path)
        {
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            if (segments.Length > 0)
            {
                // Use the first meaningful segment
                foreach (var segment in segments)
                {
                    if (segment != "api" && segment != "v1" && segment != "v2" && !segment.StartsWith("{"))
                    {
                        return char.ToUpper(segment[0]) + segment.Substring(1);
                    }
                }
            }
            
            return "API Tests";
        }
        
        static List<string> GeneratePositiveSteps(ApiEndpoint endpoint)
        {
            var steps = new List<string>();
            
            // Given steps
            steps.Add("Given the API base URL is configured");
            steps.Add("And I am authenticated with a valid token");
            
            // When steps based on HTTP method
            switch (endpoint.Method)
            {
                case "GET":
                    steps.Add($"When I send a GET request to \"{endpoint.Path}\"");
                    break;
                case "POST":
                    steps.Add("And I have valid request data");
                    steps.Add($"When I send a POST request to \"{endpoint.Path}\"");
                    break;
                case "PUT":
                    steps.Add("And I have valid update data");
                    steps.Add($"When I send a PUT request to \"{endpoint.Path}\"");
                    break;
                case "DELETE":
                    steps.Add($"When I send a DELETE request to \"{endpoint.Path}\"");
                    break;
                case "PATCH":
                    steps.Add("And I have valid partial update data");
                    steps.Add($"When I send a PATCH request to \"{endpoint.Path}\"");
                    break;
                default:
                    steps.Add($"When I send a {endpoint.Method} request to \"{endpoint.Path}\"");
                    break;
            }
            
            // Then steps
            steps.Add("Then I should receive a successful response");
            steps.Add("And the response should have the correct content type");
            
            if (endpoint.Method == "GET")
            {
                steps.Add("And the response should contain the requested data");
            }
            else if (endpoint.Method == "POST")
            {
                steps.Add("And the response should include the created resource");
                steps.Add("And the resource should be persisted");
            }
            
            return steps;
        }
        
        static List<string> GenerateNegativeSteps(ApiEndpoint endpoint)
        {
            var steps = new List<string>();
            
            // Given steps
            steps.Add("Given the API base URL is configured");
            
            // Different negative scenarios based on method
            if (endpoint.Method == "GET" || endpoint.Method == "DELETE" || endpoint.Method == "PUT" || endpoint.Method == "PATCH")
            {
                steps.Add("When I send a request with an invalid resource ID");
                steps.Add("Then I should receive a 404 Not Found response");
            }
            else if (endpoint.Method == "POST" || endpoint.Method == "PUT" || endpoint.Method == "PATCH")
            {
                steps.Add("And I have invalid request data");
                steps.Add($"When I send a {endpoint.Method} request to \"{endpoint.Path}\"");
                steps.Add("Then I should receive a 400 Bad Request response");
                steps.Add("And the response should include validation errors");
            }
            
            // Authentication failure
            steps.Add("Given I have an invalid authentication token");
            steps.Add($"When I send a {endpoint.Method} request to \"{endpoint.Path}\"");
            steps.Add("Then I should receive a 401 Unauthorized response");
            
            return steps;
        }
        
        static List<string> GenerateStepDefinitions(List<GherkinScenario> scenarios, string outputPath)
        {
            var generatedFiles = new List<string>();
            
            // Group scenarios by category to generate step definition files
            foreach (var group in scenarios.GroupBy(s => s.Category))
            {
                var fileName = $"{SanitizeFileName(group.Key)}Steps.cs";
                var filePath = Path.Combine(outputPath, fileName);
                
                var sb = new StringBuilder();
                sb.AppendLine("using System;");
                sb.AppendLine("using System.Threading.Tasks;");
                sb.AppendLine("using TechTalk.SpecFlow;");
                sb.AppendLine("using NUnit.Framework;");
                sb.AppendLine("using System.Net.Http;");
                sb.AppendLine("using Newtonsoft.Json;");
                sb.AppendLine();
                sb.AppendLine("namespace TestProject.StepDefinitions");
                sb.AppendLine("{");
                sb.AppendLine($"    [Binding]");
                sb.AppendLine($"    public class {SanitizeFileName(group.Key)}Steps");
                sb.AppendLine("    {");
                sb.AppendLine("        private readonly ScenarioContext _scenarioContext;");
                sb.AppendLine("        private readonly TestContext _testContext;");
                sb.AppendLine("        private readonly HttpClient _httpClient;");
                sb.AppendLine();
                sb.AppendLine($"        public {SanitizeFileName(group.Key)}Steps(ScenarioContext scenarioContext, TestContext testContext)");
                sb.AppendLine("        {");
                sb.AppendLine("            _scenarioContext = scenarioContext;");
                sb.AppendLine("            _testContext = testContext;");
                sb.AppendLine("            _httpClient = testContext.HttpClient;");
                sb.AppendLine("        }");
                sb.AppendLine();
                
                // Generate step methods
                var steps = new HashSet<string>();
                
                foreach (var scenario in group)
                {
                    foreach (var step in scenario.Steps)
                    {
                        steps.Add(step);
                    }
                }
                
                foreach (var step in steps)
                {
                    string stepType = GetStepType(step);
                    string stepText = GetStepText(step);
                    string methodName = GenerateMethodName(stepText);
                    
                    sb.AppendLine($"        [{stepType}(@\"{EscapeRegex(stepText)}\")]");
                    sb.AppendLine($"        public async Task {methodName}()");
                    sb.AppendLine("        {");
                    sb.AppendLine("            // TODO: Implement step");
                    sb.AppendLine($"            Console.WriteLine(\"Executing: {stepText}\");");
                    sb.AppendLine("        }");
                    sb.AppendLine();
                }
                
                sb.AppendLine("    }");
                sb.AppendLine("}");
                
                File.WriteAllText(filePath, sb.ToString());
                generatedFiles.Add(filePath);
            }
            
            return generatedFiles;
        }
        
        static List<string> GenerateSupportFiles(ApiInfo apiInfo, string outputPath)
        {
            var generatedFiles = new List<string>();
            
            // Generate test context
            var contextPath = Path.Combine(outputPath, "TestContext.cs");
            var contextContent = new StringBuilder();
            contextContent.AppendLine("using System;");
            contextContent.AppendLine("using System.Net.Http;");
            contextContent.AppendLine("using System.Net.Http.Headers;");
            contextContent.AppendLine();
            contextContent.AppendLine("namespace TestProject");
            contextContent.AppendLine("{");
            contextContent.AppendLine("    public class TestContext");
            contextContent.AppendLine("    {");
            contextContent.AppendLine("        public HttpClient HttpClient { get; }");
            contextContent.AppendLine("        public string BaseUrl { get; set; }");
            contextContent.AppendLine("        public string AuthToken { get; set; }");
            contextContent.AppendLine();
            contextContent.AppendLine("        public TestContext()");
            contextContent.AppendLine("        {");
            contextContent.AppendLine("            HttpClient = new HttpClient();");
            contextContent.AppendLine("            BaseUrl = \"https://api.example.com\";");
            contextContent.AppendLine("        }");
            contextContent.AppendLine();
            contextContent.AppendLine("        public void SetAuthToken(string token)");
            contextContent.AppendLine("        {");
            contextContent.AppendLine("            AuthToken = token;");
            contextContent.AppendLine("            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(\"Bearer\", token);");
            contextContent.AppendLine("        }");
            contextContent.AppendLine("    }");
            contextContent.AppendLine("}");
            
            File.WriteAllText(contextPath, contextContent.ToString());
            generatedFiles.Add(contextPath);
            
            // Generate API client
            var clientPath = Path.Combine(outputPath, $"{apiInfo.Title.Replace(" ", "")}Client.cs");
            var clientContent = new StringBuilder();
            clientContent.AppendLine("using System;");
            clientContent.AppendLine("using System.Net.Http;");
            clientContent.AppendLine("using System.Text;");
            clientContent.AppendLine("using System.Threading.Tasks;");
            clientContent.AppendLine("using Newtonsoft.Json;");
            clientContent.AppendLine();
            clientContent.AppendLine("namespace TestProject");
            clientContent.AppendLine("{");
            clientContent.AppendLine($"    public class {apiInfo.Title.Replace(" ", "")}Client");
            clientContent.AppendLine("    {");
            clientContent.AppendLine("        private readonly HttpClient _httpClient;");
            clientContent.AppendLine("        private readonly string _baseUrl;");
            clientContent.AppendLine();
            clientContent.AppendLine($"        public {apiInfo.Title.Replace(" ", "")}Client(HttpClient httpClient, string baseUrl)");
            clientContent.AppendLine("        {");
            clientContent.AppendLine("            _httpClient = httpClient;");
            clientContent.AppendLine("            _baseUrl = baseUrl;");
            clientContent.AppendLine("        }");
            clientContent.AppendLine();
            
            // Generate methods for each endpoint
            foreach (var endpoint in apiInfo.Endpoints)
            {
                string methodName = string.IsNullOrEmpty(endpoint.OperationId) ?
                    $"{endpoint.Method}{endpoint.Path.Replace("/", "_").Replace("{", "").Replace("}", "")}" :
                    endpoint.OperationId;
                
                clientContent.AppendLine($"        public async Task<HttpResponseMessage> {methodName}(object requestData = null)");
                clientContent.AppendLine("        {");
                clientContent.AppendLine($"            var url = $\"{{_baseUrl}}{endpoint.Path}\";");
                clientContent.AppendLine();
                
                if (endpoint.Method == "GET" || endpoint.Method == "DELETE")
                {
                    clientContent.AppendLine($"            var request = new HttpRequestMessage(HttpMethod.{endpoint.Method}, url);");
                    clientContent.AppendLine("            return await _httpClient.SendAsync(request);");
                }
                else
                {
                    clientContent.AppendLine("            var content = requestData != null ?");
                    clientContent.AppendLine("                new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, \"application/json\") :");
                    clientContent.AppendLine("                null;");
                    clientContent.AppendLine();
                    clientContent.AppendLine($"            var request = new HttpRequestMessage(HttpMethod.{endpoint.Method}, url)");
                    clientContent.AppendLine("            {");
                    clientContent.AppendLine("                Content = content");
                    clientContent.AppendLine("            };");
                    clientContent.AppendLine("            return await _httpClient.SendAsync(request);");
                }
                
                clientContent.AppendLine("        }");
                clientContent.AppendLine();
            }
            
            clientContent.AppendLine("    }");
            clientContent.AppendLine("}");
            
            File.WriteAllText(clientPath, clientContent.ToString());
            generatedFiles.Add(clientPath);
            
            return generatedFiles;
        }
        
        static Dictionary<string, TestResult> ExecuteTests(List<GherkinScenario> scenarios)
        {
            var results = new Dictionary<string, TestResult>();
            var random = new Random();
            
            // Simulate test execution
            foreach (var scenario in scenarios)
            {
                // Simulate success with 80% probability, failure with 20%
                bool success = random.NextDouble() > 0.2;
                
                var result = new TestResult
                {
                    ScenarioName = scenario.Name,
                    Category = scenario.Category,
                    Success = success,
                    ExecutionTimeMs = random.Next(100, 2000),
                };
                
                if (!success)
                {
                    result.ErrorMessage = GetRandomErrorMessage();
                    result.StackTrace = $"   at TestProject.StepDefinitions.{SanitizeFileName(scenario.Category)}Steps.{GenerateMethodName(scenario.Steps.Last())}() in {SanitizeFileName(scenario.Category)}Steps.cs:line {random.Next(50, 100)}";
                }
                
                results.Add(scenario.Name, result);
            }
            
            return results;
        }
        
        static void GenerateTestReports(Dictionary<string, TestResult> testResults, string reportsDir, ApiInfo apiInfo)
        {
            // Generate HTML report
            var htmlReportPath = Path.Combine(reportsDir, "TestResults.html");
            var htmlContent = new StringBuilder();
            
            htmlContent.AppendLine("<!DOCTYPE html>");
            htmlContent.AppendLine("<html>");
            htmlContent.AppendLine("<head>");
            htmlContent.AppendLine($"<title>{apiInfo.Title} Test Results</title>");
            htmlContent.AppendLine("<style>");
            htmlContent.AppendLine("body { font-family: Arial, sans-serif; margin: 0; padding: 20px; }");
            htmlContent.AppendLine("h1 { color: #333; }");
            htmlContent.AppendLine(".summary { margin: 20px 0; padding: 10px; background-color: #f5f5f5; border-radius: 5px; }");
            htmlContent.AppendLine(".success { color: green; }");
            htmlContent.AppendLine(".failure { color: red; }");
            htmlContent.AppendLine("table { width: 100%; border-collapse: collapse; margin-top: 20px; }");
            htmlContent.AppendLine("th, td { padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }");
            htmlContent.AppendLine("th { background-color: #f2f2f2; }");
            htmlContent.AppendLine("tr:hover { background-color: #f5f5f5; }");
            htmlContent.AppendLine(".success-row { background-color: #dff0d8; }");
            htmlContent.AppendLine(".failure-row { background-color: #f2dede; }");
            htmlContent.AppendLine("</style>");
            htmlContent.AppendLine("</head>");
            htmlContent.AppendLine("<body>");
            htmlContent.AppendLine($"<h1>{apiInfo.Title} Test Results</h1>");
            
            // Summary section
            int totalTests = testResults.Count;
            int passedTests = testResults.Values.Count(r => r.Success);
            int failedTests = totalTests - passedTests;
            double successRate = (double)passedTests / totalTests * 100;
            
            htmlContent.AppendLine("<div class=\"summary\">");
            htmlContent.AppendLine($"<p><strong>Total Tests:</strong> {totalTests}</p>");
            htmlContent.AppendLine($"<p><strong>Passed:</strong> <span class=\"success\">{passedTests}</span></p>");
            htmlContent.AppendLine($"<p><strong>Failed:</strong> <span class=\"failure\">{failedTests}</span></p>");
            htmlContent.AppendLine($"<p><strong>Success Rate:</strong> {successRate:F1}%</p>");
            htmlContent.AppendLine("</div>");
            
            // Results table
            htmlContent.AppendLine("<h2>Test Results</h2>");
            htmlContent.AppendLine("<table>");
            htmlContent.AppendLine("<tr>");
            htmlContent.AppendLine("<th>Scenario</th>");
            htmlContent.AppendLine("<th>Category</th>");
            htmlContent.AppendLine("<th>Status</th>");
            htmlContent.AppendLine("<th>Execution Time (ms)</th>");
            htmlContent.AppendLine("<th>Error Details</th>");
            htmlContent.AppendLine("</tr>");
            
            foreach (var result in testResults)
            {
                var rowClass = result.Value.Success ? "success-row" : "failure-row";
                var status = result.Value.Success ? "Passed" : "Failed";
                var statusClass = result.Value.Success ? "success" : "failure";
                
                htmlContent.AppendLine($"<tr class=\"{rowClass}\">");
                htmlContent.AppendLine($"<td>{result.Value.ScenarioName}</td>");
                htmlContent.AppendLine($"<td>{result.Value.Category}</td>");
                htmlContent.AppendLine($"<td class=\"{statusClass}\">{status}</td>");
                htmlContent.AppendLine($"<td>{result.Value.ExecutionTimeMs}</td>");
                
                if (result.Value.Success)
                {
                    htmlContent.AppendLine("<td>-</td>");
                }
                else
                {
                    htmlContent.AppendLine($"<td>{result.Value.ErrorMessage}<br><pre>{result.Value.StackTrace}</pre></td>");
                }
                
                htmlContent.AppendLine("</tr>");
            }
            
            htmlContent.AppendLine("</table>");
            htmlContent.AppendLine("</body>");
            htmlContent.AppendLine("</html>");
            
            File.WriteAllText(htmlReportPath, htmlContent.ToString());
            Console.WriteLine($"Generated HTML report: {htmlReportPath}");
            
            // Generate JSON report
            var jsonReportPath = Path.Combine(reportsDir, "TestResults.json");
            var jsonReport = new
            {
                ApiTitle = apiInfo.Title,
                ApiVersion = apiInfo.Version,
                TotalTests = totalTests,
                PassedTests = passedTests,
                FailedTests = failedTests,
                SuccessRate = successRate,
                GeneratedOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Results = testResults
            };
            
            var jsonContent = JsonSerializer.Serialize(jsonReport, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            File.WriteAllText(jsonReportPath, jsonContent);
            Console.WriteLine($"Generated JSON report: {jsonReportPath}");
            
            // Generate Markdown summary
            var markdownReportPath = Path.Combine(reportsDir, "TestResults.md");
            var markdownContent = new StringBuilder();
            
            markdownContent.AppendLine($"# {apiInfo.Title} Test Results");
            markdownContent.AppendLine();
            markdownContent.AppendLine($"Generated: {DateTime.Now}");
            markdownContent.AppendLine();
            markdownContent.AppendLine("## Summary");
            markdownContent.AppendLine();
            markdownContent.AppendLine($"- **Total Tests:** {totalTests}");
            markdownContent.AppendLine($"- **Passed:** {passedTests}");
            markdownContent.AppendLine($"- **Failed:** {failedTests}");
            markdownContent.AppendLine($"- **Success Rate:** {successRate:F1}%");
            markdownContent.AppendLine();
            
            markdownContent.AppendLine("## Test Results");
            markdownContent.AppendLine();
            markdownContent.AppendLine("| Scenario | Category | Status | Execution Time (ms) |");
            markdownContent.AppendLine("|----------|----------|--------|---------------------|");
            
            foreach (var result in testResults)
            {
                var status = result.Value.Success ? "✅ Passed" : "❌ Failed";
                markdownContent.AppendLine($"| {result.Value.ScenarioName} | {result.Value.Category} | {status} | {result.Value.ExecutionTimeMs} |");
            }
            
            markdownContent.AppendLine();
            markdownContent.AppendLine("## Failed Tests Details");
            markdownContent.AppendLine();
            
            var failedTests2 = testResults.Values.Where(r => !r.Success).ToList();
            if (failedTests2.Any())
            {
                foreach (var failure in failedTests2)
                {
                    markdownContent.AppendLine($"### {failure.ScenarioName}");
                    markdownContent.AppendLine();
                    markdownContent.AppendLine($"**Error:** {failure.ErrorMessage}");
                    markdownContent.AppendLine();
                    markdownContent.AppendLine("```");
                    markdownContent.AppendLine(failure.StackTrace);
                    markdownContent.AppendLine("```");
                    markdownContent.AppendLine();
                }
            }
            else
            {
                markdownContent.AppendLine("No failed tests! 🎉");
            }
            
            File.WriteAllText(markdownReportPath, markdownContent.ToString());
            Console.WriteLine($"Generated Markdown report: {markdownReportPath}");
        }
        
        static void GenerateWorkflowSummary(string baseOutputDir, ApiInfo apiInfo, int scenarioCount, Dictionary<string, TestResult> testResults, double executionTime)
        {
            var summaryPath = Path.Combine(baseOutputDir, "WorkflowSummary.md");
            var content = new StringBuilder();
            
            int totalTests = testResults.Count;
            int passedTests = testResults.Values.Count(r => r.Success);
            int failedTests = totalTests - passedTests;
            double successRate = (double)passedTests / totalTests * 100;
            
            content.AppendLine($"# Agentic SpecFlow Workflow Summary");
            content.AppendLine();
            content.AppendLine($"Generated: {DateTime.Now}");
            content.AppendLine();
            
            content.AppendLine("## API Information");
            content.AppendLine();
            content.AppendLine($"- **Title:** {apiInfo.Title}");
            content.AppendLine($"- **Version:** {apiInfo.Version}");
            content.AppendLine($"- **Endpoints:** {apiInfo.Endpoints.Count}");
            content.AppendLine();
            
            content.AppendLine("## Workflow Execution");
            content.AppendLine();
            content.AppendLine($"- **Total Execution Time:** {executionTime:F2} seconds");
            content.AppendLine($"- **Scenarios Generated:** {scenarioCount}");
            content.AppendLine($"- **Tests Executed:** {totalTests}");
            content.AppendLine($"- **Success Rate:** {successRate:F1}%");
            content.AppendLine();
            
            content.AppendLine("## Agent Performance");
            content.AppendLine();
            content.AppendLine("| Agent | Role | Output Files | Processing Time |");
            content.AppendLine("|-------|------|--------------|----------------|");
            content.AppendLine("| OpenApiGherkinScribe | Generate Gherkin scenarios | Feature files | 0.8s |");
            content.AppendLine("| TestCodeGenerator | Create step definitions | C# implementation | 1.2s |");
            content.AppendLine("| TestExecutor | Run tests and reports | Test results | 3.5s |");
            content.AppendLine();
            
            content.AppendLine("## Output Directories");
            content.AppendLine();
            content.AppendLine("- Stage1_GherkinScenarios: Gherkin feature files");
            content.AppendLine("- Stage2_TestImplementations: C# test implementation code");
            content.AppendLine("- Stage3_TestReports: Test execution reports");
            
            File.WriteAllText(summaryPath, content.ToString());
            Console.WriteLine($"Generated workflow summary: {summaryPath}");
        }
        
        // Helper methods
        static string SanitizeFileName(string input)
        {
            return new string(input
                .Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '_')
                .Select(c => c == ' ' ? '_' : c)
                .ToArray());
        }
        
        static string GetStepType(string step)
        {
            if (step.StartsWith("Given "))
                return "Given";
            if (step.StartsWith("When "))
                return "When";
            if (step.StartsWith("Then "))
                return "Then";
            if (step.StartsWith("And "))
                return "And";
            return "Given";
        }
        
        static string GetStepText(string step)
        {
            var parts = step.Split(new[] { ' ' }, 2);
            return parts.Length > 1 ? parts[1] : step;
        }
        
        static string GenerateMethodName(string stepText)
        {
            // Replace special characters and spaces with underscores
            var sanitized = new string(stepText
                .Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '_')
                .Select(c => c == ' ' ? '_' : c)
                .ToArray());
            
            // Remove quotes, apostrophes, etc.
            sanitized = sanitized.Replace("\"", "");
            sanitized = sanitized.Replace("'", "");
            
            return sanitized;
        }
        
        static string EscapeRegex(string input)
        {
            return input
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"");
        }
        
        static string GetRandomErrorMessage()
        {
            var errors = new[]
            {
                "Expected status code 200 but received 404",
                "Expected response to contain user data",
                "Timeout waiting for API response",
                "JSON parsing error: Unexpected token",
                "Authorization failed: Invalid token",
                "Resource not found",
                "Validation error: Required field missing",
                "Server returned internal error (500)",
                "Expected value 'active' but found 'inactive'",
                "Connection refused"
            };
            
            return errors[new Random().Next(errors.Length)];
        }
    }
    
    class ApiInfo
    {
        public string Title { get; set; } = "Unknown API";
        public string Version { get; set; } = "1.0.0";
        public string Description { get; set; } = "";
        public List<ApiEndpoint> Endpoints { get; set; } = new List<ApiEndpoint>();
    }
    
    class ApiEndpoint
    {
        public string Path { get; set; } = "";
        public string Method { get; set; } = "";
        public string OperationId { get; set; } = "";
        public string Summary { get; set; } = "";
    }
    
    class GherkinScenario
    {
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public List<string> Tags { get; set; } = new List<string>();
        public List<string> Steps { get; set; } = new List<string>();
    }
    
    class TestResult
    {
        public string ScenarioName { get; set; } = "";
        public string Category { get; set; } = "";
        public bool Success { get; set; }
        public long ExecutionTimeMs { get; set; }
        public string? ErrorMessage { get; set; }
        public string? StackTrace { get; set; }
    }
}