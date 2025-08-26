using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Packaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace McpService
{
    [Guid("18296ddc-2e02-43b6-8026-f6147188d797")]
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class McpServicePackage : AsyncPackage, ILoggerFactory, ILogger
    {
        private IComponentModel _componentModel;

        [ImportMany]
        private IEnumerable<Lazy<IMcpServerPrompt>> Prompts { get; }

        [ImportMany]
        private IEnumerable<IMcpServerResource> Resources { get; }

        [ImportMany]
        private IEnumerable<IMcpServerTool> Tools { get; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            _componentModel = await GetServiceAsync(typeof(SComponentModel)) as IComponentModel;
        }

        public ILogger CreateLogger(string? identifier = null)
        {
            return new Logger(this, identifier);
        }

        private Guid _mcpOutputPaneGuid = new Guid("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");
        private IVsOutputWindowPane? _mcpOutputPane;

        [SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Try catch block implemented")]
        public async void Log(string message)
        {
            try
            {
                await JoinableTaskFactory.SwitchToMainThreadAsync();

                // Ensure our output pane exists and cache it
                if (_mcpOutputPane == null)
                {
                    var outputWindow = await GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;
                    if (outputWindow != null)
                    {
                        if (outputWindow.GetPane(ref _mcpOutputPaneGuid, out _mcpOutputPane) < 0)
                        {
                            if (outputWindow.CreatePane(ref _mcpOutputPaneGuid, "MCP Service", fInitVisible: 1, fClearWithSolution: 1) >= 0)
                            {
                                outputWindow.GetPane(ref _mcpOutputPaneGuid, out _mcpOutputPane);
                            }
                        }

                        _mcpOutputPane.Activate();
                    }
                }

                _mcpOutputPane?.OutputString(message);
            }
            catch (Exception ex)
            {
                Debug.Fail($"Log failed for '{message}' with exception {ex.ToString()}");
            }
        }
    }
}
