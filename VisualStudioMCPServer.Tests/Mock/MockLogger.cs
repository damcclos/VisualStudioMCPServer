using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualStudioMCPServer;

namespace VisualStudioMCPServer.Tests.Mock
{
    class MockLogger : ILogger
    {
        public List<string> LoggedMessages { get; } = new List<string>();

        public void Log(string message)
        {
            LoggedMessages.Add(message);
        }
    }
}
