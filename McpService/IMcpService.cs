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
        Task RegisterCapabilityProvider(IMcpCapabilityProvider provider, CancellationToken cancellationToken = default);
        Task UnregisterCapabilityProvider(IMcpCapabilityProvider provider, CancellationToken cancellationToken = default);
    }

    public class McpService : IMcpService
    {
        public Task RegisterCapabilityProvider(IMcpCapabilityProvider provider, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
        public Task UnregisterCapabilityProvider(IMcpCapabilityProvider provider, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
