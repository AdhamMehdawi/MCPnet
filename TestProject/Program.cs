using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MCPnet.Core.Interfaces;
using MCPnet.Client.Extensions;

namespace TestProject
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("MCPnet Test Application");
            Console.WriteLine("=======================");

            // Set up dependency injection
            var services = new ServiceCollection();
            services.AddMcpServices();
            var serviceProvider = services.BuildServiceProvider();

            // Get the registry client
            var registry = serviceProvider.GetService<IMcpRegistry>();
            var client = serviceProvider.GetService<IMcpClient>();

            Console.WriteLine("Initialized MCPnet services successfully");
            Console.WriteLine($"Registry client type: {registry?.GetType().Name}");
            Console.WriteLine($"MCP client type: {client?.GetType().Name}");

            // In a real application, you would use these clients to search for and
            // communicate with MCP servers

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
