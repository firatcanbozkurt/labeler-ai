using OpcUaServer;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting OPC UA CSV Server...");
        
        var server = new CsvOpcUaServer();
        
        try
        {
            await server.StartAsync();
            Console.WriteLine("OPC UA Server started successfully!");
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting server: {ex.Message}");
        }
        finally
        {
            await server.StopAsync();
            Console.WriteLine("Server stopped.");
        }
    }
}