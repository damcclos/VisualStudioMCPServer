using McpService.Core;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.Extensions.DependencyInjection;

public static class IMcpServerBuilderExtensions
{
    public static IMcpServerBuilder WithMcpService(this IMcpServerBuilder builder, IMcpService service)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.Services.AddSingleton(service);
        builder.Services.AddHostedService<McpProvider>();
        return builder;
    }

    public static IMcpServerBuilder Configure(this IMcpServerBuilder builder, Action<McpProviderOptions> configureOptions)
    {
        builder.Services.Configure(configureOptions);
        return builder;
    }
}
