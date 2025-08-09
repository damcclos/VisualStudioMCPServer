using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualStudioMCPServer;

namespace VisualStudioMCPServer.Tests.Mock
{
    class MockOutputWindow : MockLogger, IOutputWindow
    {
        public ILogger CreateLogger(string identifier = null)
        {
            return new Logger(this, identifier);
        }
    }
}
