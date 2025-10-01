using System;
using System.IO;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("===== Test Console Output =====");
            Console.WriteLine("If you can see this message, console output is working");
            Console.WriteLine("Current directory: " + Directory.GetCurrentDirectory());
            
            // Try to read the SampleOpenAPI.json file from parent directory
            var parentDir = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
            var apiFilePath = Path.Combine(parentDir!, "AgenticSpecFlowWorkflow", "SampleOpenAPI.json");
            Console.WriteLine($"Looking for OpenAPI file at: {apiFilePath}");
            
            if (File.Exists(apiFilePath))
            {
                Console.WriteLine("OpenAPI file found!");
                try 
                {
                    var content = File.ReadAllText(apiFilePath);
                    Console.WriteLine($"File content length: {content.Length} characters");
                    Console.WriteLine("First 100 characters: " + content.Substring(0, Math.Min(100, content.Length)));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading file: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("OpenAPI file NOT found!");
                
                // Check the workspace structure
                Console.WriteLine("\nListing files in parent directory:");
                var parentFiles = Directory.GetFiles(parentDir!);
                foreach (var file in parentFiles)
                {
                    Console.WriteLine($"- {file}");
                }
                
                Console.WriteLine("\nListing directories in parent directory:");
                var parentDirs = Directory.GetDirectories(parentDir!);
                foreach (var dir in parentDirs)
                {
                    Console.WriteLine($"- {dir}");
                }
            }
        }
    }
}