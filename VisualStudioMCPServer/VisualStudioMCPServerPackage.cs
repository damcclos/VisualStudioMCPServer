using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using WebSocketSharp.Server;
using Task = System.Threading.Tasks.Task;

namespace VisualStudioMCPServer
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid("18296ddc-2e02-43b6-8026-f6147188d797")]
    public sealed class VisualStudioMCPServerPackage : AsyncPackage
    {
        private const string _wsServerAddress = "ws://localhost:4444";
        private readonly WebSocketServer _wsServer = new WebSocketServer(_wsServerAddress);

        private OutputWindow _outputWindow;
        private ILogger _logger;
        private void Log(string message) => _logger.Log(message);

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            _outputWindow = new OutputWindow(this);
            _logger = _outputWindow.CreateLogger();

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            Log("Starting MCP server...");

            _wsServer.AddWebSocketService("/mcp", () =>
            {
                return new MCP.Connection(_outputWindow);
            });
            _wsServer.Start();

            Log("MCP server started successfully");
            Log($"Server URL: {_wsServerAddress}/mcp");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Log("Stopping MCP server...");

                _wsServer.Stop();
            }
            base.Dispose(disposing);
        }
    }
}
