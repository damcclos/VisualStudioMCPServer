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
    internal sealed class McpProviderService(IMcpService? mcpService, McpServerOptions options) : IMcpProvider, IHostedService
    {
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore();
            GC.SuppressFinalize(this);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task StopAsync(CancellationToken cancellationToken) => await DisposeAsyncCore(cancellationToken);

        private async ValueTask DisposeAsyncCore(CancellationToken cancellationToken = default)
        {
            if (mcpService != null)
            {
                await mcpService.UnregisterProvider(this, cancellationToken);
                mcpService = null;
            }
        }
    }
}
