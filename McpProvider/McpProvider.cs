using McpService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McpProvider
{
    internal sealed class McpProviderService(IMcpService? mcpService, IMcpServer mcpServer) : IMcpProvider, IHostedService
    {
        public async ValueTask DisposeAsync()
        {
            if (mcpService != null)
            {
                await mcpService.UnregisterProvider(this);
                mcpService = null;
            }

            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task StopAsync(CancellationToken cancellationToken) => await DisposeAsync();
    }
}
