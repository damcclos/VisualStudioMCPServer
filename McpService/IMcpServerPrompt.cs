using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace McpService
{
    public record struct PromptArgument(bool Required, string Name, string Title, string Description);

    [InheritedExport(typeof(IMcpServerPrompt))]
    public interface IMcpServerPrompt : IMcpServerPrimitive
    {
        IReadOnlyList<PromptArgument> Arguments { get; }
    }

    public abstract class McpServerPrompt : McpServerPrimitive, IMcpServerPrompt
    {
        public IReadOnlyList<PromptArgument> Arguments { get; }

        protected McpServerPrompt(string name, string? title = null, string? description = null, IEnumerable<PromptArgument>? argument = null, IEnumerable<KeyValuePair<string, object>>? metadata = null)
            : base(name, title, description, metadata)
        {
            Arguments = argument?.ToImmutableList() ?? ImmutableList<PromptArgument>.Empty;
        }

        protected McpServerPrompt(McpServerPromptOptions options)
            : this(options.Name, options.Title, options.Description, options.Arguments, options.Metadata)
        {}
    }

    public sealed class McpServerPromptOptions : McpServerPrimitiveOptions
    {
        private List<PromptArgument> m_arguments = new List<PromptArgument>();
        private HashSet<string> m_argumentNames = new HashSet<string>();
        public IEnumerable<PromptArgument> Arguments { get => m_arguments; }

        public McpServerPromptOptions(string name, string? title = null, string? description = null)
            : base(name, title, description)
        { }

        public static McpServerPromptOptions Create(string name, string? title = null, string? description = null) => new McpServerPromptOptions(name, title, description);

        public McpServerPromptOptions WithArgument(PromptArgument argument)
        {
            if (!m_argumentNames.Add(argument.Name))
            {
                throw new ArgumentException($"Duplicate argument '{argument.Name}'", nameof(argument));
            }

            m_arguments.Add(argument);
            return this;
        }

        public McpServerPromptOptions WithArgument(bool required, string name, string? title = null, string? description = null) => WithArgument(new PromptArgument(required, name, title ?? string.Empty, description ?? string.Empty));

        public McpServerPromptOptions WithArgument(string name, string? title = null, string? description = null) => WithArgument(true, name, title, description);

        public McpServerPromptOptions WithOptionalArgument(string name, string? title = null, string? description = null) => WithArgument(false, name, title, description);

        public McpServerPromptOptions WithArguments(IEnumerable<PromptArgument> arguments)
        {
            foreach (var argument in arguments)
            {
                WithArgument(argument);
            }
            return this;
        }

        public McpServerPromptOptions WithArguments(params PromptArgument[] arguments) => WithArguments(arguments.AsEnumerable());
    }
}
