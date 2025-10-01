namespace AgenticSpecFlowWorkflow.Common
{
    /// <summary>
    /// Represents an API endpoint extracted from an OpenAPI specification
    /// </summary>
    public class ApiEndpoint
    {
        /// <summary>
        /// Gets or sets the path of the endpoint
        /// </summary>
        public string Path { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the HTTP method of the endpoint
        /// </summary>
        public string Method { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the operation ID of the endpoint
        /// </summary>
        public string OperationId { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the summary of the endpoint
        /// </summary>
        public string Summary { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the description of the endpoint
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the parameters of the endpoint
        /// </summary>
        public List<ApiParameter> Parameters { get; set; } = new List<ApiParameter>();
        
        /// <summary>
        /// Gets or sets the success status code of the endpoint
        /// </summary>
        public int SuccessStatusCode { get; set; } = 200;
        
        /// <summary>
        /// Gets or sets the type of the response
        /// </summary>
        public string ResponseType { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets whether the endpoint requires authentication
        /// </summary>
        public bool RequiresAuthentication { get; set; }
    }
    
    /// <summary>
    /// Represents a parameter of an API endpoint
    /// </summary>
    public class ApiParameter
    {
        /// <summary>
        /// Gets or sets the name of the parameter
        /// </summary>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the location of the parameter
        /// </summary>
        public string In { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets the description of the parameter
        /// </summary>
        public string Description { get; set; } = string.Empty;
        
        /// <summary>
        /// Gets or sets whether the parameter is required
        /// </summary>
        public bool Required { get; set; }
        
        /// <summary>
        /// Gets or sets the type of the parameter
        /// </summary>
        public string Type { get; set; } = string.Empty;
    }
}