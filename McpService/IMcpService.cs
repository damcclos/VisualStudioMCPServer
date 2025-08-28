using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace McpService
{
    public interface SMcpService
    { }

    public interface IMcpService
    {
        Task RegisterProvider(IMcpProvider provider);
        Task UnregisterProvider(IMcpProvider provider);
    }

    internal class McpService : IHostedService
    {
        public McpService(IMcpServer mcpServer)
        { }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
