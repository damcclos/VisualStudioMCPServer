// https://github.com/modelcontextprotocol/csharp-sdk/blob/0cf449eaca3d06a28bf9a81f243077a13a91b76e/samples/TestServerWithHosting/Tools/EchoTool.cs

using ModelContextProtocol.Server;
using System.ComponentModel;

namespace BasicProvider.Tools;

[McpServerToolType]
public sealed class EchoTool
{
    [McpServerTool, Description("Echoes the input back to the client.")]
    public static string Echo(string message)
    {
        return "hello " + message;
    }
}
