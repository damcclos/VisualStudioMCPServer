using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace McpService
{
    public static class AsyncPackageExtensions
    {
        public static async ValueTask<IMcpCapabilityProviderBuilder> CreateMcpCapabilityProviderBuilderAsync(this AsyncPackage package, CancellationToken cancellationToken = default) =>
            new McpCapabilityProviderBuilder(await package.GetServiceAsync<SMcpService, IMcpService>(cancellationToken));
    }
}
