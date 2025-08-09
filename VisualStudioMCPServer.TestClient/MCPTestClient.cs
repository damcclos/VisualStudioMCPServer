using System;
using System.Threading.Tasks;
using WebSocketSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VisualStudioMCPServer.TestClient
{
    public class MCPTestClient : IDisposable
    {
        private readonly WebSocket _webSocket;
        private readonly TaskCompletionSource<bool> _connectionTcs;
        private TaskCompletionSource<string> _messageTcs;
        private bool _isDisposed;

        public bool IsConnected => _webSocket?.ReadyState == WebSocketState.Open;

        public MCPTestClient(string url)
        {
            _webSocket = new WebSocket(url);
            _connectionTcs = new TaskCompletionSource<bool>();
            _messageTcs = null;

            _webSocket.OnOpen += (sender, e) =>
            {
                Console.WriteLine("Connected to server");
                _connectionTcs.TrySetResult(true);
            };

            _webSocket.OnError += (sender, e) =>
            {
                Console.WriteLine($"Error: {e.Message}");
                _connectionTcs.TrySetException(new Exception(e.Message));
                _messageTcs.TrySetException(new Exception(e.Message));
            };

            _webSocket.OnClose += (sender, e) =>
            {
                Console.WriteLine($"Connection closed: {e.Reason}");
                _connectionTcs.TrySetResult(false);
            };

            _webSocket.OnMessage += (sender, e) =>
            {
                if (e.IsText)
                {
                    Console.WriteLine($"Received: {e.Data}");
                    _messageTcs?.TrySetResult(e.Data);
                }
            };
        }

        public async Task ConnectAsync(int timeoutMs = 5000)
        {
            _webSocket.Connect();

            using var cts = new System.Threading.CancellationTokenSource(timeoutMs);
            using var registration = cts.Token.Register(() => _connectionTcs.TrySetCanceled());
            try
            {
                await _connectionTcs.Task;
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException($"Connection attempt timed out after {timeoutMs}ms");
            }
        }

        public async Task<JObject> SendTestMessageAsync(string method, object? parameters = null, int timeoutMs = 5000)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Client is not connected");

            var request = new
            {
                jsonrpc = "2.0",
                id = Guid.NewGuid().ToString("N"),
                method,
                @params = parameters
            };

            string json = JsonConvert.SerializeObject(request);
            _messageTcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

            _webSocket.Send(json);
            Console.WriteLine($"Sent: {json}");

            using var cts = new System.Threading.CancellationTokenSource(timeoutMs);
            using var registration = cts.Token.Register(() => _messageTcs.TrySetCanceled());
            try
            {
                string response = await _messageTcs.Task;
                var j = JObject.Parse(response);
                if (j["error"] != null)
                {
                    var message = j["error"]?["message"]?.ToString() ?? "Unknown error";
                    throw new Exception(message);
                }
                return j;
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException($"Response timeout after {timeoutMs}ms");
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            if (_webSocket != null)
            {
                if (_webSocket.ReadyState == WebSocketState.Open)
                {
                    _webSocket.Close();
                }
                _webSocket.OnMessage -= (sender, e) => { };
                _webSocket.OnError -= (sender, e) => { };
                _webSocket.OnClose -= (sender, e) => { };
                _webSocket.OnOpen -= (sender, e) => { };
            }

            _isDisposed = true;
        }
    }
}