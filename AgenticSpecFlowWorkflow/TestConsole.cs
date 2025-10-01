using System;

public class TestConsole
{
    public static void Main()
    {
        Console.WriteLine("===== Test Console Output =====");
        Console.WriteLine("If you can see this message, console output is working");
        Console.WriteLine("Current directory: " + System.IO.Directory.GetCurrentDirectory());
        
        // Try to read the SampleOpenAPI.json file
        var apiFilePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "SampleOpenAPI.json");
        Console.WriteLine($"Looking for OpenAPI file at: {apiFilePath}");
        
        if (System.IO.File.Exists(apiFilePath))
        {
            Console.WriteLine("OpenAPI file found!");
            try 
            {
                var content = System.IO.File.ReadAllText(apiFilePath);
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
        }
    }
}