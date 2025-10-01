using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgenticSpecFlowWorkflow.Common
{
    /// <summary>
    /// Generator for test reports
    /// </summary>
    public class ReportGenerator
    {
        /// <summary>
        /// Generates a summary report of test results
        /// </summary>
        public async Task GenerateSummaryReportAsync(string outputPath, IEnumerable<TestRunResult> results)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\">");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine("    <title>Test Summary Report</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; color: #333; }");
            sb.AppendLine("        h1 { color: #0066cc; }");
            sb.AppendLine("        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }");
            sb.AppendLine("        th, td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }");
            sb.AppendLine("        th { background-color: #0066cc; color: white; }");
            sb.AppendLine("        tr:hover { background-color: #f5f5f5; }");
            sb.AppendLine("        .success { color: green; }");
            sb.AppendLine("        .failure { color: red; }");
            sb.AppendLine("        .warning { color: orange; }");
            sb.AppendLine("        .summary-card { background-color: #f9f9f9; border-radius: 5px; padding: 15px; margin-bottom: 20px; border-left: 5px solid #0066cc; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            
            sb.AppendLine("    <h1>Test Summary Report</h1>");
            sb.AppendLine("    <div class=\"summary-card\">");
            sb.AppendLine($"        <p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
            
            var totalTests = results.Sum(r => r.TotalTests);
            var totalPassed = results.Sum(r => r.Passed);
            var totalFailed = results.Sum(r => r.Failed);
            var totalSkipped = results.Sum(r => r.Skipped);
            var overallPassRate = totalTests == 0 ? 0 : (double)totalPassed / totalTests * 100;
            
            sb.AppendLine($"        <p><strong>Total Tests:</strong> {totalTests}</p>");
            sb.AppendLine($"        <p><strong>Passed:</strong> <span class=\"success\">{totalPassed} ({overallPassRate:F2}%)</span></p>");
            sb.AppendLine($"        <p><strong>Failed:</strong> <span class=\"failure\">{totalFailed}</span></p>");
            sb.AppendLine($"        <p><strong>Skipped:</strong> <span class=\"warning\">{totalSkipped}</span></p>");
            sb.AppendLine("    </div>");
            
            sb.AppendLine("    <h2>Results by Category</h2>");
            sb.AppendLine("    <table>");
            sb.AppendLine("        <tr>");
            sb.AppendLine("            <th>Category</th>");
            sb.AppendLine("            <th>Total</th>");
            sb.AppendLine("            <th>Passed</th>");
            sb.AppendLine("            <th>Failed</th>");
            sb.AppendLine("            <th>Skipped</th>");
            sb.AppendLine("            <th>Pass Rate</th>");
            sb.AppendLine("            <th>Duration</th>");
            sb.AppendLine("        </tr>");
            
            foreach (var result in results)
            {
                sb.AppendLine("        <tr>");
                sb.AppendLine($"            <td>{result.Category}</td>");
                sb.AppendLine($"            <td>{result.TotalTests}</td>");
                sb.AppendLine($"            <td class=\"success\">{result.Passed}</td>");
                sb.AppendLine($"            <td class=\"failure\">{result.Failed}</td>");
                sb.AppendLine($"            <td class=\"warning\">{result.Skipped}</td>");
                sb.AppendLine($"            <td>{result.PassRate:F2}%</td>");
                sb.AppendLine($"            <td>{result.ExecutionTimeMs / 1000.0:F2} seconds</td>");
                sb.AppendLine("        </tr>");
            }
            
            sb.AppendLine("    </table>");
            
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            await File.WriteAllTextAsync(outputPath, sb.ToString());
        }
        
        /// <summary>
        /// Generates a detailed report of test results
        /// </summary>
        public async Task GenerateDetailedReportAsync(string outputPath, IEnumerable<TestRunResult> results)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\">");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine("    <title>Detailed Test Report</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; color: #333; }");
            sb.AppendLine("        h1, h2, h3 { color: #0066cc; }");
            sb.AppendLine("        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }");
            sb.AppendLine("        th, td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }");
            sb.AppendLine("        th { background-color: #0066cc; color: white; }");
            sb.AppendLine("        tr:hover { background-color: #f5f5f5; }");
            sb.AppendLine("        .success { color: green; }");
            sb.AppendLine("        .failure { color: red; }");
            sb.AppendLine("        .warning { color: orange; }");
            sb.AppendLine("        .summary-card { background-color: #f9f9f9; border-radius: 5px; padding: 15px; margin-bottom: 20px; border-left: 5px solid #0066cc; }");
            sb.AppendLine("        .log-container { background-color: #f5f5f5; padding: 15px; border-radius: 5px; margin: 20px 0; overflow-x: auto; }");
            sb.AppendLine("        .log-entry { font-family: monospace; white-space: pre-wrap; }");
            sb.AppendLine("        .category-section { margin-bottom: 40px; }");
            sb.AppendLine("        .accordion { background-color: #eee; color: #444; cursor: pointer; padding: 18px; width: 100%; text-align: left; border: none; outline: none; transition: 0.4s; margin-bottom: 5px; }");
            sb.AppendLine("        .active, .accordion:hover { background-color: #ddd; }");
            sb.AppendLine("        .panel { padding: 0 18px; background-color: white; max-height: 0; overflow: hidden; transition: max-height 0.2s ease-out; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            
            sb.AppendLine("    <h1>Detailed Test Report</h1>");
            sb.AppendLine("    <div class=\"summary-card\">");
            sb.AppendLine($"        <p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
            
            var totalTests = results.Sum(r => r.TotalTests);
            var totalPassed = results.Sum(r => r.Passed);
            var totalFailed = results.Sum(r => r.Failed);
            var totalSkipped = results.Sum(r => r.Skipped);
            var overallPassRate = totalTests == 0 ? 0 : (double)totalPassed / totalTests * 100;
            
            sb.AppendLine($"        <p><strong>Total Tests:</strong> {totalTests}</p>");
            sb.AppendLine($"        <p><strong>Passed:</strong> <span class=\"success\">{totalPassed} ({overallPassRate:F2}%)</span></p>");
            sb.AppendLine($"        <p><strong>Failed:</strong> <span class=\"failure\">{totalFailed}</span></p>");
            sb.AppendLine($"        <p><strong>Skipped:</strong> <span class=\"warning\">{totalSkipped}</span></p>");
            sb.AppendLine("    </div>");
            
            foreach (var result in results)
            {
                sb.AppendLine($"    <div class=\"category-section\">");
                sb.AppendLine($"        <h2>{result.Category} Tests</h2>");
                sb.AppendLine($"        <p>Execution Time: {result.ExecutionTimeMs / 1000.0:F2} seconds</p>");
                sb.AppendLine($"        <p>Pass Rate: {result.PassRate:F2}%</p>");
                
                sb.AppendLine($"        <button class=\"accordion\">View Test Log</button>");
                sb.AppendLine($"        <div class=\"panel\">");
                sb.AppendLine($"            <div class=\"log-container\">");
                sb.AppendLine($"                <pre class=\"log-entry\">{result.DetailedLog}</pre>");
                sb.AppendLine($"            </div>");
                sb.AppendLine($"        </div>");
                sb.AppendLine($"    </div>");
            }
            
            // Add JavaScript for accordion functionality
            sb.AppendLine("    <script>");
            sb.AppendLine("        var acc = document.getElementsByClassName(\"accordion\");");
            sb.AppendLine("        var i;");
            sb.AppendLine("        ");
            sb.AppendLine("        for (i = 0; i < acc.length; i++) {");
            sb.AppendLine("          acc[i].addEventListener(\"click\", function() {");
            sb.AppendLine("            this.classList.toggle(\"active\");");
            sb.AppendLine("            var panel = this.nextElementSibling;");
            sb.AppendLine("            if (panel.style.maxHeight) {");
            sb.AppendLine("              panel.style.maxHeight = null;");
            sb.AppendLine("            } else {");
            sb.AppendLine("              panel.style.maxHeight = panel.scrollHeight + \"px\";");
            sb.AppendLine("            }");
            sb.AppendLine("          });");
            sb.AppendLine("        }");
            sb.AppendLine("    </script>");
            
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            await File.WriteAllTextAsync(outputPath, sb.ToString());
        }
        
        /// <summary>
        /// Generates a coverage report based on test results and API contract
        /// </summary>
        public async Task GenerateCoverageReportAsync(string outputPath, string openApiContractPath, IEnumerable<TestRunResult> results)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang=\"en\">");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset=\"UTF-8\">");
            sb.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine("    <title>Test Coverage Report</title>");
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: Arial, sans-serif; margin: 0; padding: 20px; color: #333; }");
            sb.AppendLine("        h1, h2 { color: #0066cc; }");
            sb.AppendLine("        table { border-collapse: collapse; width: 100%; margin-bottom: 20px; }");
            sb.AppendLine("        th, td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }");
            sb.AppendLine("        th { background-color: #0066cc; color: white; }");
            sb.AppendLine("        tr:hover { background-color: #f5f5f5; }");
            sb.AppendLine("        .high-coverage { color: green; }");
            sb.AppendLine("        .medium-coverage { color: orange; }");
            sb.AppendLine("        .low-coverage { color: red; }");
            sb.AppendLine("        .summary-card { background-color: #f9f9f9; border-radius: 5px; padding: 15px; margin-bottom: 20px; border-left: 5px solid #0066cc; }");
            sb.AppendLine("        .coverage-bar-container { width: 100%; background-color: #e0e0e0; border-radius: 4px; }");
            sb.AppendLine("        .coverage-bar { height: 20px; background-color: #4CAF50; border-radius: 4px; }");
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            
            sb.AppendLine("    <h1>Test Coverage Report</h1>");
            sb.AppendLine("    <div class=\"summary-card\">");
            sb.AppendLine($"        <p><strong>Generated:</strong> {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
            sb.AppendLine($"        <p><strong>API Contract:</strong> {Path.GetFileName(openApiContractPath)}</p>");
            
            // In a real implementation, we would analyze the test coverage based on the API contract
            // For this demo, we'll use mock data
            
            sb.AppendLine("        <h2>Overall Coverage</h2>");
            sb.AppendLine("        <div class=\"coverage-bar-container\">");
            sb.AppendLine("            <div class=\"coverage-bar\" style=\"width: 78%\"></div>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        <p><strong>78% Covered</strong></p>");
            sb.AppendLine("    </div>");
            
            sb.AppendLine("    <h2>Endpoint Coverage</h2>");
            sb.AppendLine("    <table>");
            sb.AppendLine("        <tr>");
            sb.AppendLine("            <th>Endpoint</th>");
            sb.AppendLine("            <th>Method</th>");
            sb.AppendLine("            <th>Coverage</th>");
            sb.AppendLine("            <th>Test Count</th>");
            sb.AppendLine("            <th>Status</th>");
            sb.AppendLine("        </tr>");
            
            // Mock endpoint coverage data
            var endpointCoverage = new List<(string Endpoint, string Method, int Coverage, int TestCount)>
            {
                ("/api/users", "GET", 90, 3),
                ("/api/users/{id}", "GET", 85, 4),
                ("/api/users", "POST", 80, 3),
                ("/api/users/{id}", "PUT", 70, 2),
                ("/api/users/{id}", "DELETE", 65, 2),
                ("/api/products", "GET", 50, 1),
                ("/api/products/{id}", "GET", 30, 1),
                ("/api/orders", "POST", 0, 0)
            };
            
            foreach (var coverage in endpointCoverage)
            {
                string coverageClass;
                if (coverage.Coverage >= 80)
                    coverageClass = "high-coverage";
                else if (coverage.Coverage >= 50)
                    coverageClass = "medium-coverage";
                else
                    coverageClass = "low-coverage";
                
                sb.AppendLine("        <tr>");
                sb.AppendLine($"            <td>{coverage.Endpoint}</td>");
                sb.AppendLine($"            <td>{coverage.Method}</td>");
                sb.AppendLine($"            <td>");
                sb.AppendLine($"                <div class=\"coverage-bar-container\">");
                sb.AppendLine($"                    <div class=\"coverage-bar\" style=\"width: {coverage.Coverage}%\"></div>");
                sb.AppendLine($"                </div>");
                sb.AppendLine($"            </td>");
                sb.AppendLine($"            <td>{coverage.TestCount}</td>");
                sb.AppendLine($"            <td class=\"{coverageClass}\">{coverage.Coverage}%</td>");
                sb.AppendLine("        </tr>");
            }
            
            sb.AppendLine("    </table>");
            
            sb.AppendLine("    <h2>Coverage by Test Category</h2>");
            sb.AppendLine("    <table>");
            sb.AppendLine("        <tr>");
            sb.AppendLine("            <th>Category</th>");
            sb.AppendLine("            <th>Endpoints Covered</th>");
            sb.AppendLine("            <th>Coverage</th>");
            sb.AppendLine("        </tr>");
            
            // Mock category coverage data
            var categoryCoverage = new List<(string Category, int EndpointsCovered, int TotalEndpoints, int Coverage)>
            {
                ("Acceptance", 6, 8, 75),
                ("Integration", 5, 8, 62),
                ("Live", 4, 8, 50)
            };
            
            foreach (var coverage in categoryCoverage)
            {
                string coverageClass;
                if (coverage.Coverage >= 80)
                    coverageClass = "high-coverage";
                else if (coverage.Coverage >= 50)
                    coverageClass = "medium-coverage";
                else
                    coverageClass = "low-coverage";
                
                sb.AppendLine("        <tr>");
                sb.AppendLine($"            <td>{coverage.Category}</td>");
                sb.AppendLine($"            <td>{coverage.EndpointsCovered} of {coverage.TotalEndpoints}</td>");
                sb.AppendLine($"            <td class=\"{coverageClass}\">{coverage.Coverage}%</td>");
                sb.AppendLine("        </tr>");
            }
            
            sb.AppendLine("    </table>");
            
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));
            await File.WriteAllTextAsync(outputPath, sb.ToString());
        }
    }
}