using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Extensibility;

namespace McpService;

[VisualStudioContribution]
internal class ExtensionEntrypoint : Extension
{
    private readonly McpService m_mcpService = new();

    public override ExtensionConfiguration ExtensionConfiguration => new()
    {
        Metadata = new(
                id: "McpService.9b926609-0f03-4ecb-aa3c-c59ba0be98b0",
                version: this.ExtensionAssemblyVersion,
                publisherName: "Publisher name",
                displayName: "McpService",
                description: "Extension description"),
        LoadedWhen = ActivationConstraint.SolutionState(SolutionState.NoSolution),
    };

    protected override void InitializeServices(IServiceCollection services)
    {
        base.InitializeServices(services);

        services.AddMcpServer()
            .WithHttpTransport()
            .WithCallToolHandler((request, cancellationToken) => m_mcpService.CallToolAsync(request, cancellationToken))
            .WithCompleteHandler((request, cancellationToken) => m_mcpService.CompleteAsync(request, cancellationToken))
            .WithGetPromptHandler((request, cancellationToken) => m_mcpService.GetPromptAsync(request, cancellationToken))
            .WithListPromptsHandler((request, cancellationToken) => m_mcpService.ListPromptsAsync(request, cancellationToken))
            .WithListResourcesHandler((request, cancellationToken) => m_mcpService.ListResourcesAsync(request, cancellationToken))
            .WithListResourceTemplatesHandler((request, cancellationToken) => m_mcpService.ListResourceTemplatesAsync(request, cancellationToken))
            .WithListToolsHandler((request, cancellationToken) => m_mcpService.ListToolsAsync(request, cancellationToken))
            .WithReadResourceHandler((request, cancellationToken) => m_mcpService.ReadResourceAsync(request, cancellationToken))
            .WithSubscribeToResourcesHandler((request, cancellationToken) => m_mcpService.SubscribeToResourcesAsync(request, cancellationToken))
            .WithUnsubscribeFromResourcesHandler((request, cancellationToken) => m_mcpService.UnsubscribeFromResourcesAsync(request, cancellationToken));
    }
}
