using System.Text;

namespace AgenticSpecFlowWorkflow.Common
{
    /// <summary>
    /// Factory for generating test code
    /// </summary>
    public class TestCodeFactory
    {
        /// <summary>
        /// Generates a step definition class for the given steps
        /// </summary>
        public string GenerateStepDefinitionClass(string className, List<string> steps)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Net.Http;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using NUnit.Framework;");
            sb.AppendLine("using TechTalk.SpecFlow;");
            sb.AppendLine();
            sb.AppendLine("namespace SpecflowTests.StepDefinitions");
            sb.AppendLine("{");
            sb.AppendLine($"    [Binding]");
            sb.AppendLine($"    public class {className}");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly TestContext _context;");
            sb.AppendLine();
            sb.AppendLine($"        public {className}(TestContext context)");
            sb.AppendLine("        {");
            sb.AppendLine("            _context = context;");
            sb.AppendLine("        }");
            sb.AppendLine();
            
            foreach (var step in steps)
            {
                string stepType = GetStepType(step);
                string stepText = GetStepText(step);
                string methodName = GetMethodName(step);
                
                sb.AppendLine($"        [{stepType}(@\"{EscapeRegex(stepText)}\")]");
                sb.AppendLine($"        public void {methodName}()");
                sb.AppendLine("        {");
                sb.AppendLine($"            // TODO: Implement step definition for: {step}");
                sb.AppendLine("            throw new PendingStepException();");
                sb.AppendLine("        }");
                sb.AppendLine();
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// Generates a test context class
        /// </summary>
        public string GenerateTestContextClass()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Net.Http;");
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine();
            sb.AppendLine("namespace SpecflowTests");
            sb.AppendLine("{");
            sb.AppendLine("    public class TestContext");
            sb.AppendLine("    {");
            sb.AppendLine("        public HttpClient HttpClient { get; } = new HttpClient();");
            sb.AppendLine();
            sb.AppendLine("        public Dictionary<string, object> Variables { get; } = new Dictionary<string, object>();");
            sb.AppendLine();
            sb.AppendLine("        public HttpResponseMessage? Response { get; set; }");
            sb.AppendLine();
            sb.AppendLine("        public T GetVariable<T>(string name)");
            sb.AppendLine("        {");
            sb.AppendLine("            if (Variables.TryGetValue(name, out var value))");
            sb.AppendLine("            {");
            sb.AppendLine("                if (value is T typedValue)");
            sb.AppendLine("                {");
            sb.AppendLine("                    return typedValue;");
            sb.AppendLine("                }");
            sb.AppendLine();
            sb.AppendLine("                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(value));");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            throw new KeyNotFoundException($\"Variable '{name}' not found in test context\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public void SetVariable(string name, object value)");
            sb.AppendLine("        {");
            sb.AppendLine("            Variables[name] = value;");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// Generates an API client class based on OpenAPI specification
        /// </summary>
        public string GenerateApiClientClass(string openApiContractPath)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Net.Http;");
            sb.AppendLine("using System.Net.Http.Headers;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using Newtonsoft.Json;");
            sb.AppendLine();
            sb.AppendLine("namespace IntegrationTests");
            sb.AppendLine("{");
            sb.AppendLine("    public class ApiClient : IDisposable");
            sb.AppendLine("    {");
            sb.AppendLine("        private readonly HttpClient _httpClient;");
            sb.AppendLine("        private readonly string _baseUrl;");
            sb.AppendLine("        private string? _authToken;");
            sb.AppendLine();
            sb.AppendLine("        public ApiClient(string baseUrl)");
            sb.AppendLine("        {");
            sb.AppendLine("            _baseUrl = baseUrl;");
            sb.AppendLine("            _httpClient = new HttpClient();");
            sb.AppendLine("            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(\"application/json\"));");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public void SetAuthToken(string token)");
            sb.AppendLine("        {");
            sb.AppendLine("            _authToken = token;");
            sb.AppendLine("            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(\"Bearer\", token);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public async Task<T?> GetAsync<T>(string endpoint, Dictionary<string, string>? queryParams = null)");
            sb.AppendLine("        {");
            sb.AppendLine("            var url = BuildUrl(endpoint, queryParams);");
            sb.AppendLine("            var response = await _httpClient.GetAsync(url);");
            sb.AppendLine("            response.EnsureSuccessStatusCode();");
            sb.AppendLine("            var content = await response.Content.ReadAsStringAsync();");
            sb.AppendLine("            return JsonConvert.DeserializeObject<T>(content);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public async Task<T?> PostAsync<T>(string endpoint, object body)");
            sb.AppendLine("        {");
            sb.AppendLine("            var url = BuildUrl(endpoint);");
            sb.AppendLine("            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, \"application/json\");");
            sb.AppendLine("            var response = await _httpClient.PostAsync(url, content);");
            sb.AppendLine("            response.EnsureSuccessStatusCode();");
            sb.AppendLine("            var responseContent = await response.Content.ReadAsStringAsync();");
            sb.AppendLine("            return JsonConvert.DeserializeObject<T>(responseContent);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public async Task<T?> PutAsync<T>(string endpoint, object body)");
            sb.AppendLine("        {");
            sb.AppendLine("            var url = BuildUrl(endpoint);");
            sb.AppendLine("            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, \"application/json\");");
            sb.AppendLine("            var response = await _httpClient.PutAsync(url, content);");
            sb.AppendLine("            response.EnsureSuccessStatusCode();");
            sb.AppendLine("            var responseContent = await response.Content.ReadAsStringAsync();");
            sb.AppendLine("            return JsonConvert.DeserializeObject<T>(responseContent);");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public async Task DeleteAsync(string endpoint)");
            sb.AppendLine("        {");
            sb.AppendLine("            var url = BuildUrl(endpoint);");
            sb.AppendLine("            var response = await _httpClient.DeleteAsync(url);");
            sb.AppendLine("            response.EnsureSuccessStatusCode();");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        private string BuildUrl(string endpoint, Dictionary<string, string>? queryParams = null)");
            sb.AppendLine("        {");
            sb.AppendLine("            var url = $\"{_baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}\";");
            sb.AppendLine();
            sb.AppendLine("            if (queryParams != null && queryParams.Count > 0)");
            sb.AppendLine("            {");
            sb.AppendLine("                var query = string.Join(\"&\", queryParams.Select(p => $\"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}\"));");
            sb.AppendLine("                url = $\"{url}?{query}\";");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine("            return url;");
            sb.AppendLine("        }");
            sb.AppendLine();
            sb.AppendLine("        public void Dispose()");
            sb.AppendLine("        {");
            sb.AppendLine("            _httpClient.Dispose();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// Generates model classes based on OpenAPI specification
        /// </summary>
        public List<string> GenerateModelClasses(string openApiContractPath, string outputDir)
        {
            var generatedFiles = new List<string>();
            
            // In a real implementation, we would parse the OpenAPI spec to generate model classes
            // For this demo, we'll create some sample model classes
            
            // User model
            var userModelPath = Path.Combine(outputDir, "User.cs");
            var userModelContent = @"namespace IntegrationTests.Models
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}";
            File.WriteAllText(userModelPath, userModelContent);
            generatedFiles.Add(userModelPath);
            
            // CreateUserRequest model
            var createUserRequestPath = Path.Combine(outputDir, "CreateUserRequest.cs");
            var createUserRequestContent = @"namespace IntegrationTests.Models
{
    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}";
            File.WriteAllText(createUserRequestPath, createUserRequestContent);
            generatedFiles.Add(createUserRequestPath);
            
            // UpdateUserRequest model
            var updateUserRequestPath = Path.Combine(outputDir, "UpdateUserRequest.cs");
            var updateUserRequestContent = @"namespace IntegrationTests.Models
{
    public class UpdateUserRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}";
            File.WriteAllText(updateUserRequestPath, updateUserRequestContent);
            generatedFiles.Add(updateUserRequestPath);
            
            return generatedFiles;
        }
        
        /// <summary>
        /// Generates an integration test class for the given scenarios
        /// </summary>
        public string GenerateIntegrationTestClass(string className, List<GherkinScenario> scenarios)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Net.Http;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using NUnit.Framework;");
            sb.AppendLine("using IntegrationTests.Models;");
            sb.AppendLine();
            sb.AppendLine("namespace IntegrationTests");
            sb.AppendLine("{");
            sb.AppendLine($"    [TestFixture]");
            sb.AppendLine($"    public class {className}");
            sb.AppendLine("    {");
            sb.AppendLine("        private ApiClient _apiClient;");
            sb.AppendLine();
            sb.AppendLine("        [SetUp]");
            sb.AppendLine("        public void Setup()");
            sb.AppendLine("        {");
            sb.AppendLine("            var baseUrl = TestConfig.GetApiBaseUrl();");
            sb.AppendLine("            _apiClient = new ApiClient(baseUrl);");
            sb.AppendLine();
            sb.AppendLine("            if (TestConfig.RequiresAuthentication())");
            sb.AppendLine("            {");
            sb.AppendLine("                var token = TestConfig.GetAuthToken();");
            sb.AppendLine("                _apiClient.SetAuthToken(token);");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
            sb.AppendLine();
            
            foreach (var scenario in scenarios)
            {
                string testName = scenario.ScenarioName.Replace(" ", "");
                string tags = string.Join(", ", scenario.Tags.Select(t => $"\"{t}\""));
                
                sb.AppendLine($"        [Test]");
                if (!string.IsNullOrEmpty(tags))
                {
                    sb.AppendLine($"        [Category({tags})]");
                }
                sb.AppendLine($"        public async Task {testName}()");
                sb.AppendLine("        {");
                sb.AppendLine($"            // TODO: Implement test for scenario: {scenario.ScenarioName}");
                sb.AppendLine($"            // Steps:");
                foreach (var step in scenario.Steps)
                {
                    sb.AppendLine($"            // - {step}");
                }
                sb.AppendLine("            Assert.Fail(\"Test not implemented\");");
                sb.AppendLine("        }");
                sb.AppendLine();
            }
            
            sb.AppendLine("        [TearDown]");
            sb.AppendLine("        public void TearDown()");
            sb.AppendLine("        {");
            sb.AppendLine("            _apiClient.Dispose();");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// Generates a live test class for the given scenarios
        /// </summary>
        public string GenerateLiveTestClass(string className, List<GherkinScenario> scenarios)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Net.Http;");
            sb.AppendLine("using System.Threading.Tasks;");
            sb.AppendLine("using NUnit.Framework;");
            sb.AppendLine("using IntegrationTests.Models;");
            sb.AppendLine();
            sb.AppendLine("namespace LiveTests");
            sb.AppendLine("{");
            sb.AppendLine($"    [TestFixture]");
            sb.AppendLine($"    public class {className}");
            sb.AppendLine("    {");
            sb.AppendLine("        private ApiClient _apiClient;");
            sb.AppendLine("        private TestLogger _logger;");
            sb.AppendLine();
            sb.AppendLine("        [SetUp]");
            sb.AppendLine("        public void Setup()");
            sb.AppendLine("        {");
            sb.AppendLine("            var baseUrl = TestConfig.GetLiveApiBaseUrl();");
            sb.AppendLine("            _apiClient = new ApiClient(baseUrl);");
            sb.AppendLine("            _logger = new TestLogger($\"{GetType().Name}\");");
            sb.AppendLine();
            sb.AppendLine("            if (TestConfig.RequiresAuthentication())");
            sb.AppendLine("            {");
            sb.AppendLine("                var token = TestConfig.GetLiveAuthToken();");
            sb.AppendLine("                _apiClient.SetAuthToken(token);");
            sb.AppendLine("            }");
            sb.AppendLine("            ");
            sb.AppendLine("            _logger.LogInfo(\"Test setup completed\");");
            sb.AppendLine("        }");
            sb.AppendLine();
            
            foreach (var scenario in scenarios)
            {
                string testName = scenario.ScenarioName.Replace(" ", "");
                string tags = string.Join(", ", scenario.Tags.Select(t => $"\"{t}\""));
                
                sb.AppendLine($"        [Test]");
                if (!string.IsNullOrEmpty(tags))
                {
                    sb.AppendLine($"        [Category({tags})]");
                }
                sb.AppendLine($"        public async Task {testName}()");
                sb.AppendLine("        {");
                sb.AppendLine($"            _logger.LogInfo($\"Starting test: {scenario.ScenarioName}\");");
                sb.AppendLine();
                sb.AppendLine($"            try");
                sb.AppendLine("            {");
                sb.AppendLine($"                // TODO: Implement live test for scenario: {scenario.ScenarioName}");
                sb.AppendLine($"                // Steps:");
                foreach (var step in scenario.Steps)
                {
                    sb.AppendLine($"                // - {step}");
                }
                sb.AppendLine();
                sb.AppendLine("                Assert.Fail(\"Test not implemented\");");
                sb.AppendLine("            }");
                sb.AppendLine("            catch (Exception ex)");
                sb.AppendLine("            {");
                sb.AppendLine("                _logger.LogError($\"Test failed: {ex.Message}\");");
                sb.AppendLine("                throw;");
                sb.AppendLine("            }");
                sb.AppendLine("            finally");
                sb.AppendLine("            {");
                sb.AppendLine("                _logger.LogInfo(\"Test completed\");");
                sb.AppendLine("            }");
                sb.AppendLine("        }");
                sb.AppendLine();
            }
            
            sb.AppendLine("        [TearDown]");
            sb.AppendLine("        public void TearDown()");
            sb.AppendLine("        {");
            sb.AppendLine("            _apiClient.Dispose();");
            sb.AppendLine("            _logger.LogInfo(\"Test teardown completed\");");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// Generates a test configuration file
        /// </summary>
        public string GenerateTestConfigurationFile()
        {
            return @"{
  ""ApiSettings"": {
    ""BaseUrl"": ""https://api.example.com/v1"",
    ""LiveBaseUrl"": ""https://api.example.com/v1"",
    ""RequiresAuthentication"": true,
    ""AuthToken"": ""sample-auth-token"",
    ""LiveAuthToken"": ""sample-live-auth-token""
  },
  ""TestSettings"": {
    ""RetryCount"": 3,
    ""TimeoutSeconds"": 30,
    ""ParallelExecution"": true,
    ""ScreenshotsEnabled"": true,
    ""ScreenshotsPath"": ""./TestResults/Screenshots""
  },
  ""LogSettings"": {
    ""LogLevel"": ""Information"",
    ""LogPath"": ""./TestResults/Logs"",
    ""ConsoleOutput"": true
  }
}";
        }
        
        /// <summary>
        /// Generates a SpecFlow configuration file
        /// </summary>
        public string GenerateSpecFlowConfigFile()
        {
            return @"{
  ""bindingCulture"": {
    ""name"": ""en-us""
  },
  ""language"": {
    ""feature"": ""en""
  },
  ""unitTestProvider"": {
    ""name"": ""NUnit""
  },
  ""runtime"": {
    ""missingOrPendingStepsOutcome"": ""Inconclusive""
  },
  ""plugins"": [
    {
      ""name"": ""NUnit""
    }
  ]
}";
        }
        
        private string GetStepType(string step)
        {
            if (step.StartsWith("Given"))
                return "Given";
            if (step.StartsWith("When"))
                return "When";
            if (step.StartsWith("Then"))
                return "Then";
            if (step.StartsWith("And") || step.StartsWith("But"))
                return "And";
            
            return "Given";
        }
        
        private string GetStepText(string step)
        {
            // Extract the step text after the step type
            var parts = step.Split(new[] { ' ' }, 2);
            return parts.Length > 1 ? parts[1] : step;
        }
        
        private string GetMethodName(string step)
        {
            // Convert step text to a valid method name
            string sanitized = step
                .Replace("\"", "")
                .Replace("'", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("{", "")
                .Replace("}", "")
                .Replace("[", "")
                .Replace("]", "")
                .Replace("<", "")
                .Replace(">", "")
                .Replace(".", "")
                .Replace(",", "")
                .Replace(";", "")
                .Replace(":", "")
                .Replace("/", "")
                .Replace("\\", "")
                .Replace("-", "_")
                .Replace(" ", "");
            
            // Ensure the first character is uppercase
            if (!string.IsNullOrEmpty(sanitized))
            {
                sanitized = char.ToUpper(sanitized[0]) + sanitized.Substring(1);
            }
            
            return sanitized;
        }
        
        private string EscapeRegex(string input)
        {
            return input
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace(".", "\\.")
                .Replace("$", "\\$")
                .Replace("^", "\\^")
                .Replace("{", "\\{")
                .Replace("}", "\\}")
                .Replace("[", "\\[")
                .Replace("]", "\\]")
                .Replace("(", "\\(")
                .Replace(")", "\\)")
                .Replace("|", "\\|")
                .Replace("*", "\\*")
                .Replace("+", "\\+")
                .Replace("?", "\\?");
        }
    }
}