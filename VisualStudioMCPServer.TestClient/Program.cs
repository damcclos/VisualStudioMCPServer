using System;
using System.Threading.Tasks;

namespace VisualStudioMCPServer.TestClient
{
    class Program
    {
        private const string ServerUrl = "ws://localhost:4444/mcp";
        private const int ConnectionTimeoutMs = 5000;
        private const int ResponseTimeoutMs = 5000;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Visual Studio MCP Test Client");
            Console.WriteLine("-----------------------------");

            using var client = new MCPTestClient(ServerUrl);
            
            try
            {
                // Test 1: Connection
                Console.WriteLine("\nTest 1: Connecting to server...");
                await client.ConnectAsync(ConnectionTimeoutMs);
                Console.WriteLine("✓ Connection successful");

                // Test 2: Basic Message Exchange
                Console.WriteLine("\nTest 2: Testing basic message exchange...");
                var response = await client.SendTestMessageAsync("ping", new { message = "hello" }, ResponseTimeoutMs);
                Console.WriteLine("✓ Message exchange successful");

                // Test 3: Invalid Method
                Console.WriteLine("\nTest 3: Testing invalid method handling...");
                try
                {
                    await client.SendTestMessageAsync("invalid_method", new { }, ResponseTimeoutMs);
                    Console.WriteLine("✗ Expected error response for invalid method");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("✓ Invalid method properly handled: " + ex.Message);
                }

                Console.WriteLine("\nAll tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }
}