using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AgenticSpecFlowWorkflow.Common;

namespace AgenticSpecFlowWorkflow.Agents
{
    /// <summary>
    /// TestExecutor - An agent that runs test cases and generates test reports
    /// </summary>
    public class TestExecutor : IAgent
    {
        private readonly ILogger<TestExecutor> _logger;
        private readonly TestRunner _testRunner;
        private readonly ReportGenerator _reportGenerator;

        public string Name => "TestExecutor";
        
        public string Description => "Agent that executes end-to-end test scenarios and live tests, and generates comprehensive test reports";

        public TestExecutor(ILogger<TestExecutor> logger, TestRunner testRunner, ReportGenerator reportGenerator)
        {
            _logger = logger;
            _testRunner = testRunner;
            _reportGenerator = reportGenerator;
        }

        public async Task<AgentResult> ExecuteAsync(AgentContext context)
        {
            _logger.LogInformation($"{Name} starting execution");
            
            try
            {
                // Step 1: Read the test implementations created by the previous agent
                if (!context.Input.ContainsKey("TestImplementationsPath") || 
                    !context.Input.ContainsKey("AcceptanceTestsPath") || 
                    !context.Input.ContainsKey("IntegrationTestsPath") ||
                    !context.Input.ContainsKey("LiveTestsPath"))
                {
                    throw new InvalidOperationException("Required input from previous agent not found");
                }
                
                var testImplPath = context.Input["TestImplementationsPath"] as string;
                var acceptanceTestsPath = context.Input["AcceptanceTestsPath"] as string;
                var integrationTestsPath = context.Input["IntegrationTestsPath"] as string;
                var liveTestsPath = context.Input["LiveTestsPath"] as string;
                
                _logger.LogInformation($"Found test implementations at: {testImplPath}");
                
                // Step 2: Create reports directory
                var reportsPath = Path.Combine(context.OutputDirectory, "TestReports");
                Directory.CreateDirectory(reportsPath);
                
                _logger.LogInformation($"Created test reports directory at: {reportsPath}");
                
                // Step 3: Execute acceptance tests
                _logger.LogInformation("Executing acceptance tests...");
                var acceptanceTestResults = await _testRunner.RunAcceptanceTestsAsync(acceptanceTestsPath);
                _logger.LogInformation($"Acceptance tests completed: {acceptanceTestResults.TotalTests} tests, {acceptanceTestResults.Passed} passed, {acceptanceTestResults.Failed} failed");
                
                // Step 4: Execute integration tests
                _logger.LogInformation("Executing integration tests...");
                var integrationTestResults = await _testRunner.RunIntegrationTestsAsync(integrationTestsPath);
                _logger.LogInformation($"Integration tests completed: {integrationTestResults.TotalTests} tests, {integrationTestResults.Passed} passed, {integrationTestResults.Failed} failed");
                
                // Step 5: Execute live tests
                _logger.LogInformation("Executing live tests...");
                var liveTestResults = await _testRunner.RunLiveTestsAsync(liveTestsPath);
                _logger.LogInformation($"Live tests completed: {liveTestResults.TotalTests} tests, {liveTestResults.Passed} passed, {liveTestResults.Failed} failed");
                
                // Step 6: Generate test result logs
                var acceptanceTestLogFile = Path.Combine(reportsPath, "acceptance-test-log.txt");
                await File.WriteAllTextAsync(acceptanceTestLogFile, acceptanceTestResults.DetailedLog);
                
                var integrationTestLogFile = Path.Combine(reportsPath, "integration-test-log.txt");
                await File.WriteAllTextAsync(integrationTestLogFile, integrationTestResults.DetailedLog);
                
                var liveTestLogFile = Path.Combine(reportsPath, "live-test-log.txt");
                await File.WriteAllTextAsync(liveTestLogFile, liveTestResults.DetailedLog);
                
                // Step 7: Generate consolidated test reports
                var summaryReportFile = Path.Combine(reportsPath, "test-summary-report.html");
                await _reportGenerator.GenerateSummaryReportAsync(
                    summaryReportFile, 
                    new[] { acceptanceTestResults, integrationTestResults, liveTestResults }
                );
                
                var detailedReportFile = Path.Combine(reportsPath, "test-detailed-report.html");
                await _reportGenerator.GenerateDetailedReportAsync(
                    detailedReportFile,
                    new[] { acceptanceTestResults, integrationTestResults, liveTestResults }
                );
                
                // Step 8: Generate test coverage report
                var coverageReportFile = Path.Combine(reportsPath, "test-coverage-report.html");
                await _reportGenerator.GenerateCoverageReportAsync(
                    coverageReportFile,
                    context.OpenApiContractPath,
                    new[] { acceptanceTestResults, integrationTestResults, liveTestResults }
                );
                
                _logger.LogInformation($"Generated test reports at: {reportsPath}");
                
                // Return successful result with the output locations
                return new AgentResult
                {
                    Success = true,
                    Output = new Dictionary<string, object>
                    {
                        { "ReportsPath", reportsPath },
                        { "SummaryReport", summaryReportFile },
                        { "DetailedReport", detailedReportFile },
                        { "CoverageReport", coverageReportFile },
                        { "TestResultsSummary", new
                            {
                                AcceptanceTests = new 
                                {
                                    Total = acceptanceTestResults.TotalTests,
                                    Passed = acceptanceTestResults.Passed,
                                    Failed = acceptanceTestResults.Failed,
                                    PassRate = acceptanceTestResults.PassRate
                                },
                                IntegrationTests = new 
                                {
                                    Total = integrationTestResults.TotalTests,
                                    Passed = integrationTestResults.Passed,
                                    Failed = integrationTestResults.Failed,
                                    PassRate = integrationTestResults.PassRate
                                },
                                LiveTests = new 
                                {
                                    Total = liveTestResults.TotalTests,
                                    Passed = liveTestResults.Passed,
                                    Failed = liveTestResults.Failed,
                                    PassRate = liveTestResults.PassRate
                                },
                                Overall = new
                                {
                                    Total = acceptanceTestResults.TotalTests + integrationTestResults.TotalTests + liveTestResults.TotalTests,
                                    Passed = acceptanceTestResults.Passed + integrationTestResults.Passed + liveTestResults.Passed,
                                    Failed = acceptanceTestResults.Failed + integrationTestResults.Failed + liveTestResults.Failed,
                                    PassRate = (acceptanceTestResults.Passed + integrationTestResults.Passed + liveTestResults.Passed) / 
                                              (double)(acceptanceTestResults.TotalTests + integrationTestResults.TotalTests + liveTestResults.TotalTests) * 100
                                }
                            }
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {Name}: {ex.Message}");
                return new AgentResult
                {
                    Success = false,
                    ErrorMessage = $"Error executing tests and generating reports: {ex.Message}"
                };
            }
        }
    }
}