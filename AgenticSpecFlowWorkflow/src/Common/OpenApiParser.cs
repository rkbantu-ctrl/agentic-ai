using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AgenticSpecFlowWorkflow.Common
{
    /// <summary>
    /// Parser for OpenAPI specifications
    /// </summary>
    public class OpenApiParser
    {
        private readonly ILogger<OpenApiParser> _logger;
        
        public OpenApiParser(ILogger<OpenApiParser> logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Parses an OpenAPI specification file
        /// </summary>
        public async Task<dynamic> ParseAsync(string filePath)
        {
            try
            {
                Console.WriteLine($"OpenApiParser: Attempting to parse file {filePath}");
                
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"ERROR: OpenAPI file does not exist: {filePath}");
                    throw new FileNotFoundException($"OpenAPI specification file not found: {filePath}");
                }
                
                Console.WriteLine($"OpenApiParser: File exists, reading content...");
                string content = await File.ReadAllTextAsync(filePath);
                Console.WriteLine($"OpenApiParser: File read successfully. Content length: {content.Length} characters");
                
                if (filePath.EndsWith(".json"))
                {
                    Console.WriteLine($"OpenApiParser: Parsing JSON content...");
                    var jsonOptions = new JsonSerializerOptions
                    {
                        AllowTrailingCommas = true,
                        PropertyNameCaseInsensitive = true
                    };
                    
                    try {
                        var result = JsonSerializer.Deserialize<JsonDocument>(content);
                        Console.WriteLine($"OpenApiParser: JSON parsed successfully!");
                        return result!; // null-forgiving operator since we're checking for exceptions
                    }
                    catch (JsonException jsonEx)
                    {
                        Console.WriteLine($"ERROR: JSON parsing error: {jsonEx.Message}");
                        _logger.LogError(jsonEx, $"JSON parsing error: {jsonEx.Message}");
                        throw;
                    }
                }
                else if (filePath.EndsWith(".yaml") || filePath.EndsWith(".yml"))
                {
                    // Convert YAML to JSON and then deserialize
                    // In a real implementation, use a YAML parser library like YamlDotNet
                    Console.WriteLine("WARNING: YAML parsing is not fully implemented in this demo");
                    _logger.LogWarning("YAML parsing is a simplified implementation for demo purposes");
                    throw new NotImplementedException("YAML parsing is not implemented in this demo");
                }
                else
                {
                    Console.WriteLine($"ERROR: Unsupported file format: {Path.GetExtension(filePath)}");
                    throw new InvalidOperationException($"Unsupported OpenAPI file format: {Path.GetExtension(filePath)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR in OpenApiParser: {ex.GetType().Name}: {ex.Message}");
                _logger.LogError(ex, $"Error parsing OpenAPI spec: {ex.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// Extracts API endpoints from an OpenAPI specification
        /// </summary>
        public List<ApiEndpoint> ExtractEndpoints(dynamic openApiSpec)
        {
            var endpoints = new List<ApiEndpoint>();
            
            // In a real implementation, we would parse the OpenAPI spec to extract the endpoints
            // This is a simplified implementation for demo purposes
            _logger.LogInformation("Extracting endpoints from OpenAPI spec");
            
            // Simplified implementation to create some sample endpoints
            endpoints.Add(new ApiEndpoint
            {
                Path = "/api/users",
                Method = "GET",
                OperationId = "getUsers",
                Summary = "Get users",
                Description = "Returns a list of users",
                SuccessStatusCode = 200,
                ResponseType = "array",
                RequiresAuthentication = true,
                Parameters = new List<ApiParameter>
                {
                    new ApiParameter
                    {
                        Name = "page",
                        In = "query",
                        Description = "Page number",
                        Required = false,
                        Type = "integer"
                    },
                    new ApiParameter
                    {
                        Name = "limit",
                        In = "query",
                        Description = "Number of items per page",
                        Required = false,
                        Type = "integer"
                    }
                }
            });
            
            endpoints.Add(new ApiEndpoint
            {
                Path = "/api/users/{id}",
                Method = "GET",
                OperationId = "getUserById",
                Summary = "Get user by ID",
                Description = "Returns a user by ID",
                SuccessStatusCode = 200,
                ResponseType = "object",
                RequiresAuthentication = true,
                Parameters = new List<ApiParameter>
                {
                    new ApiParameter
                    {
                        Name = "id",
                        In = "path",
                        Description = "User ID",
                        Required = true,
                        Type = "string"
                    }
                }
            });
            
            endpoints.Add(new ApiEndpoint
            {
                Path = "/api/users",
                Method = "POST",
                OperationId = "createUser",
                Summary = "Create user",
                Description = "Creates a new user",
                SuccessStatusCode = 201,
                ResponseType = "object",
                RequiresAuthentication = true,
                Parameters = new List<ApiParameter>
                {
                    new ApiParameter
                    {
                        Name = "body",
                        In = "body",
                        Description = "User data",
                        Required = true,
                        Type = "object"
                    }
                }
            });
            
            endpoints.Add(new ApiEndpoint
            {
                Path = "/api/users/{id}",
                Method = "PUT",
                OperationId = "updateUser",
                Summary = "Update user",
                Description = "Updates an existing user",
                SuccessStatusCode = 200,
                ResponseType = "object",
                RequiresAuthentication = true,
                Parameters = new List<ApiParameter>
                {
                    new ApiParameter
                    {
                        Name = "id",
                        In = "path",
                        Description = "User ID",
                        Required = true,
                        Type = "string"
                    },
                    new ApiParameter
                    {
                        Name = "body",
                        In = "body",
                        Description = "User data",
                        Required = true,
                        Type = "object"
                    }
                }
            });
            
            endpoints.Add(new ApiEndpoint
            {
                Path = "/api/users/{id}",
                Method = "DELETE",
                OperationId = "deleteUser",
                Summary = "Delete user",
                Description = "Deletes a user",
                SuccessStatusCode = 204,
                ResponseType = "null",
                RequiresAuthentication = true,
                Parameters = new List<ApiParameter>
                {
                    new ApiParameter
                    {
                        Name = "id",
                        In = "path",
                        Description = "User ID",
                        Required = true,
                        Type = "string"
                    }
                }
            });
            
            _logger.LogInformation($"Extracted {endpoints.Count} endpoints from OpenAPI spec");
            return endpoints;
        }
    }
}