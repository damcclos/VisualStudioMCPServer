using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.PlatformUI;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.Reflection;
using System.Threading.Channels;

namespace McpService.Core;

public interface SMcpProvider
{ }

public interface IMcpProvider
{
    string Name { get; }

    string? ServerInstructions { get; }
}

public class McpProvider(IMcpService service, IOptions<McpProviderOptions> options) : BackgroundService, IMcpProvider
{
    public string Name => options.Value.Name;

    public string? ServerInstructions => options.Value.ServerInstructions;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await service.RegisterAsync(this, stoppingToken).ConfigureAwait(false);

            TaskCompletionSource<object?> tcs = new();
            stoppingToken.Register(() => tcs.SetCanceled());
            await tcs.Task;
        }
        finally
        {
            await service.UnregisterAsync(this, stoppingToken).ConfigureAwait(false);
        }
    }
}

public sealed class McpProviderOptions
{
    public string? Name { get; set; }

    public string? ServerInstructions { get; set; }
}
