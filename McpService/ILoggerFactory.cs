using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace McpService
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger(string identifier = null);
    }

    public interface ILogger
    {
        void Log(string message);
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
}
