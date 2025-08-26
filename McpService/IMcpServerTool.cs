using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace McpService
{
    public record struct ToolAnnotations(bool DestructiveHint, bool IdempotentHint, bool OpenWorldHint, bool ReadOnlyHint, string Title);
    public record struct ToolSchema(string Type, IReadOnlyList<string> Required, IReadOnlyDictionary<string, object> Properties);

    public interface IMcpServerTool : IMcpServerPrimitive
    {
        ToolAnnotations? Annotations { get; }
        ToolSchema InputSchema { get; }
        ToolSchema? OutputSchema { get; }
    }

    public abstract class McpServerTool : McpServerPrimitive, IMcpServerTool
    {
        public ToolAnnotations? Annotations { get; }
        public ToolSchema InputSchema { get; }
        public ToolSchema? OutputSchema { get; }

        protected McpServerTool(string name, ToolSchema inputSchema, string? title = null, string? description = null, ToolAnnotations? annotations = null, ToolSchema? outputSchema = null, IEnumerable<KeyValuePair<string, object>>? metadata = null)
            : base(name, title, description, metadata)
        {
            Annotations = annotations;
            InputSchema = inputSchema;
            OutputSchema = outputSchema;
        }

        protected McpServerTool(McpServerToolOptions options)
            : this(options.Name, options.InputSchema, options.Title, options.Description, options.Annotations, options.OutputSchema, options.Metadata)
        {}
    }
}
