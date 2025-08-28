using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using McpProvider;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.VisualStudio.Shell;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMcpProvider(this IServiceCollection serviceCollection) => serviceCollection.AddHostedService<McpProviderService>();
}
