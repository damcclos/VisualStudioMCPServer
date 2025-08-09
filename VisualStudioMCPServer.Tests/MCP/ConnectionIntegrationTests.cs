using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using VisualStudioMCPServer.Tests.Mock;
using VisualStudioMCPServer.MCP;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace VisualStudioMCPServer.Tests.MCP
{
    [TestClass]
    public class ConnectionIntegrationTests
    {
        private WebSocketServer _server;
        private string _url;
        private MockOutputWindow _output = new MockOutputWindow();

        [TestInitialize]
        public void Setup()
        {
            int port = GetFreeTcpPort();
            _url = $"ws://127.0.0.1:{port}/mcp";
            _server = new WebSocketServer($"ws://127.0.0.1:{port}");
            _server.AddWebSocketService("/mcp", () => new Connection(_output));
            _server.Start();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _server?.Stop();
        }

        [TestMethod]
        public void Ping_ReturnsResult()
        {
            using var ws = new WebSocket(_url);
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            ws.OnMessage += (s, e) => { if (e.IsText) tcs.TrySetResult(e.Data); };
            ws.Connect();

            var req = new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = "1",
                ["method"] = "ping",
                ["params"] = new JObject { ["message"] = "hello" }
            };
            ws.Send(req.ToString());

            var received = tcs.Task.Wait(5000) ? tcs.Task.Result : null;
            Assert.IsNotNull(received, "No response received");

            var resp = JObject.Parse(received);
            Assert.AreEqual("2.0", (string)resp["jsonrpc"]);
            Assert.AreEqual("1", (string)resp["id"]);
            Assert.IsNotNull(resp["result"]);
            Assert.AreEqual("hello", (string)resp["result"]?["message"]);
        }

        [TestMethod]
        public void InvalidJson_ReturnsError()
        {
            using var ws = new WebSocket(_url);
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            ws.OnMessage += (s, e) => { if (e.IsText) tcs.TrySetResult(e.Data); };
            ws.Connect();

            ws.Send("not json");

            var received = tcs.Task.Wait(5000) ? tcs.Task.Result : null;
            Assert.IsNotNull(received, "No response received");
            var resp = JObject.Parse(received);
            Assert.IsNotNull(resp["error"]);
        }

        [TestMethod]
        public void MethodNotFound_ReturnsError()
        {
            using var ws = new WebSocket(_url);
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            ws.OnMessage += (s, e) => { if (e.IsText) tcs.TrySetResult(e.Data); };
            ws.Connect();

            var req = new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = "2",
                ["method"] = "nope"
            };
            ws.Send(req.ToString());

            var received = tcs.Task.Wait(5000) ? tcs.Task.Result : null;
            Assert.IsNotNull(received, "No response received");
            var resp = JObject.Parse(received);
            Assert.IsNotNull(resp["error"]);
            StringAssert.Contains(resp["error"]?["message"]?.ToString() ?? string.Empty, "Method not found");
        }

        [TestMethod]
        public void BinaryMessage_ReturnsError()
        {
            using var ws = new WebSocket(_url);
            var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            ws.OnMessage += (s, e) => { if (e.IsText) tcs.TrySetResult(e.Data); };
            ws.Connect();

            ws.Send(new byte[] { 1, 2, 3, 4 });

            var received = tcs.Task.Wait(5000) ? tcs.Task.Result : null;
            Assert.IsNotNull(received, "No response received");
            var resp = JObject.Parse(received);
            Assert.IsNotNull(resp["error"]);
            StringAssert.Contains(resp["error"]?["message"]?.ToString() ?? string.Empty, "Binary messages are not supported");
        }

        private static int GetFreeTcpPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }
    }
}

