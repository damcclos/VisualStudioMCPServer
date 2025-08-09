using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace VisualStudioMCPServer
{
    public interface ILogger
    {
        void Log(string message);
    }

    public interface IOutputWindow
    {
        ILogger CreateLogger(string identifier = null);
    }

    public class Logger : ILogger
    {
        private readonly ILogger _logger;
        private readonly string _identifier;

        public Logger(ILogger logger, string identifier = null)
        {
            _logger = logger;
            _identifier = identifier?.Length != 0 ? identifier : null;
        }

        public void Log(string message)
        {
            if (_identifier != null)
            {
                message = $"[{_identifier}]: {message}\n";
            }
            else
            {
                message = message + '\n';
            }

            _logger.Log(message);
        }
    }

    class OutputWindow : IOutputWindow, ILogger
    {
        private readonly AsyncPackage _package;

        private Guid _mcpOutputPaneGuid = new Guid("A1B2C3D4-E5F6-7890-ABCD-EF1234567890");
        private IVsOutputWindowPane _mcpOutputPane;

        public OutputWindow(AsyncPackage package)
        {
            _package = package;
        }

        public ILogger CreateLogger(string identifier = null)
        {
            return new Logger(this, identifier);
        }

        [SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Try catch block implemented")]
        public async void Log(string message)
        {
            try
            {
                await _package.JoinableTaskFactory.SwitchToMainThreadAsync();

                // Ensure our output pane exists and cache it
                if (_mcpOutputPane == null)
                {
                    var outputWindow = await _package.GetServiceAsync(typeof(SVsOutputWindow)) as IVsOutputWindow;
                    if (outputWindow != null)
                    {
                        if (outputWindow.GetPane(ref _mcpOutputPaneGuid, out _mcpOutputPane) < 0)
                        {
                            if (outputWindow.CreatePane(ref _mcpOutputPaneGuid, "MCP Server", fInitVisible: 1, fClearWithSolution: 1) >= 0)
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
