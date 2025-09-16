using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Shell;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace McpService
{
    public interface IMcpCapabilityProvider
    {}

    internal class McpCapabilityProvider(IMcpService mcpService, McpServerOptions options) : IMcpCapabilityProvider
    {
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            await mcpService.RegisterCapabilityProvider(this, cancellationToken);
        }
    }

    public interface IMcpCapabilityProviderBuilder
    {
        IServiceCollection Services { get; }

        ValueTask<IMcpCapabilityProvider> BuildAsync(CancellationToken cancellationToken = default);
    }

    public class McpCapabilityProviderBuilder(IMcpService mcpService) : IMcpCapabilityProviderBuilder
    {
        public IServiceCollection Services => new ServiceCollection();

        public async ValueTask<IMcpCapabilityProvider> BuildAsync(CancellationToken cancellationToken = default)
        {
            IServiceProvider serviceProvider = Services.BuildServiceProvider();
            IOptions<McpServerOptions> options = serviceProvider.GetRequiredService<IOptions<McpServerOptions>>();

            McpCapabilityProvider provider = new McpCapabilityProvider(mcpService, options.Value);
            await provider.InitializeAsync(cancellationToken);
            return provider;
        }
    }
}
