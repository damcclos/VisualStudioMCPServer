using ModelContextProtocol.Server;

namespace McpService.Core;

public interface SMcpService
{ }

public interface IMcpService
{
    Task RegisterAsync(IMcpProvider provider, CancellationToken cancellationToken = default);
    Task UnregisterAsync(IMcpProvider provider, CancellationToken cancellationToken = default);
}
