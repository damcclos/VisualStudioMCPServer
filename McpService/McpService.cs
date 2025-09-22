using McpService.Core;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace McpService;

class McpService : IMcpService
{
    private List<IMcpProvider> m_providers = new List<IMcpProvider>();

    public Task RegisterAsync(IMcpProvider provider, CancellationToken cancellationToken = default)
    {
        if (m_providers.Contains(provider))
            throw new ArgumentException("Provider already added", nameof(provider));
        m_providers.Add(provider);
        return Task.CompletedTask;
    }

    public Task UnregisterAsync(IMcpProvider provider, CancellationToken cancellationToken = default)
    {
        if (!m_providers.Contains(provider))
            throw new ArgumentException("Server not found", nameof(provider));
        m_providers.Remove(provider);
        return Task.CompletedTask;
    }

    public ValueTask<CallToolResult> CallToolAsync(RequestContext<CallToolRequestParams> request, CancellationToken cancellationToken = default)
    {
        return default;
    }

    public ValueTask<CompleteResult> CompleteAsync(RequestContext<CompleteRequestParams> request, CancellationToken cancellationToken = default)
    {
        return default;
    }

    public ValueTask<GetPromptResult> GetPromptAsync(RequestContext<GetPromptRequestParams> request, CancellationToken cancellationToken = default)
    {
        return default;
    }

    public ValueTask<ListPromptsResult> ListPromptsAsync(RequestContext<ListPromptsRequestParams> request, CancellationToken cancellationToken = default)
    {
        return default;
    }

    public ValueTask<ListResourcesResult> ListResourcesAsync(RequestContext<ListResourcesRequestParams> request, CancellationToken cancellationToken = default)
    {
        return default;
    }

    public ValueTask<ListResourceTemplatesResult> ListResourceTemplatesAsync(RequestContext<ListResourceTemplatesRequestParams> request, CancellationToken cancellationToken = default)
    {
        return default;
    }

    public ValueTask<ListToolsResult> ListToolsAsync(RequestContext<ListToolsRequestParams> request, CancellationToken cancellationToken = default)
    {
        return default;
    }

    public ValueTask<ReadResourceResult> ReadResourceAsync(RequestContext<ReadResourceRequestParams> request, CancellationToken cancellationToken = default)
    {
        return default;
    }

    public ValueTask<EmptyResult> SubscribeToResourcesAsync(RequestContext<SubscribeRequestParams> request, CancellationToken cancellationToken = default)
    {
        return default;
    }

    public ValueTask<EmptyResult> UnsubscribeFromResourcesAsync(RequestContext<UnsubscribeRequestParams> request, CancellationToken cancellationToken = default)
    {
        return default;
    }
}
