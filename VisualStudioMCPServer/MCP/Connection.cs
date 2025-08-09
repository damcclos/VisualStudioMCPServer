using System;
using System.Collections.Concurrent;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace VisualStudioMCPServer.MCP
{
    public class Connection : WebSocketBehavior
    {
        private readonly IOutputWindow _outputWindow;
        private ILogger _logger;
        private new void Log(string message) => _logger.Log(message);

        public DateTime ConnectedAt { get; }
        public DateTime LastActivity { get; private set; }

        public Connection(IOutputWindow outputWindow)
        {
            ConnectedAt = LastActivity = DateTime.UtcNow;

            _outputWindow = outputWindow;
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            _logger = _outputWindow.CreateLogger($"Connection {ID}");
            Log("Connection opened");
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Log($"Connection closed: {e.Reason}");
            base.OnClose(e);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            LastActivity = DateTime.UtcNow;

            if (!e.IsText)
            {
                SendError("Binary messages are not supported");
                return;
            }

            try
            {
                // Log the received message for debugging
                Log($"Received message: {e.Data}");

                // We'll implement message handling later
                // This will parse the JSON-RPC message and route it to the appropriate handler
                HandleMessage(e.Data);
            }
            catch (Exception ex)
            {
                SendError($"Error processing message: {ex.Message}");
            }
        }

        protected override void OnError(ErrorEventArgs e)
        {
            SendError($"WebSocket error: {e.Message}");
            base.OnError(e);
        }

        public void SendError(string message)
        {
            Log($"WebSocket error: {message}");

            var error = new
            {
                jsonrpc = "2.0",
                error = new
                {
                    code = -32000,
                    message = message
                }
            };

            Send(JsonConvert.SerializeObject(error));
        }

        private void HandleMessage(string message)
        {
            try
            {
                var j = JObject.Parse(message);
                var jsonrpc = j.Value<string>("jsonrpc");
                var id = j["id"];
                var method = j.Value<string>("method");
                var @params = j["params"];

                if (jsonrpc != "2.0" || string.IsNullOrEmpty(method))
                {
                    SendErrorWithId(id, -32600, "Invalid Request");
                    return;
                }

                switch (method)
                {
                    case "ping":
                    {
                        var response = new JObject
                        {
                            ["jsonrpc"] = "2.0",
                            ["id"] = id,
                            ["result"] = new JObject
                            {
                                ["message"] = (@params as JObject)?["message"] ?? "pong"
                            }
                        };
                        Send(response.ToString(Formatting.None));
                        return;
                    }
                    default:
                        SendErrorWithId(id, -32601, $"Method not found: {method}");
                        return;
                }
            }
            catch (JsonException)
            {
                SendError("Invalid JSON");
            }
        }

        private void SendErrorWithId(JToken id, int code, string message)
        {
            var error = new JObject
            {
                ["jsonrpc"] = "2.0",
                ["id"] = id,
                ["error"] = new JObject
                {
                    ["code"] = code,
                    ["message"] = message
                }
            };
            Send(error.ToString(Formatting.None));
        }

        private new void Send(string message)
        {
            Log($"Sending message: {message}");
            base.Send(message);
        }
    }
}