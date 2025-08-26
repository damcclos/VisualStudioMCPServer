using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace McpService
{
    public interface IMcpServerPrimitive
    {
        string Name { get; }
        string Title { get; }
        string Description { get; }

        IReadOnlyDictionary<string, object> Metadata { get; }
    }

    public abstract class McpServerPrimitive : IMcpServerPrimitive
    {
        public string Name { get; }
        public string Title { get; }
        public string Description { get; }

        public IReadOnlyDictionary<string, object> Metadata { get; }

        protected McpServerPrimitive(string name, string? title = null, string? description = null, IEnumerable<KeyValuePair<string, object>>? metadata = null)
        {
            Name = name;
            Title = title ?? string.Empty;
            Description = description ?? string.Empty;
            Metadata = metadata?.ToImmutableDictionary() ?? ImmutableDictionary<string, object>.Empty;
        }
    }

    public class McpServerPrimitiveOptions
    {
        public string Name { get; }
        public string? Title { get; }
        public string? Description { get; }

        private Dictionary<string, object> m_metadata = new Dictionary<string, object>();
        public IEnumerable<KeyValuePair<string, object>> Metadata { get => m_metadata; }

        protected McpServerPrimitiveOptions(string name, string? title = null, string? description = null)
        {
            Name = name;
            Title = title;
            Description = description;
        }

        internal void WithMetadata(string key, object value)
        {
            if (m_metadata.ContainsKey(key))
            {
                throw new ArgumentException($"Duplicate metadata '{key}'", nameof(key));
            }

            m_metadata[key] = value;
        }
    }

    public static class McpServerPrimitiveOptionsExtensions
    {
        public static T WithMetadata<T>(this T options, string key, object value) where T : McpServerPrimitiveOptions
        {
            options.WithMetadata(key, value);
            return options;
        }

        public static T WithMetadata<T>(this T options, IEnumerable<KeyValuePair<string, object>> metadata) where T : McpServerPrimitiveOptions
        {
            foreach (var data in metadata)
            {
                options.WithMetadata(data.Key, data.Value);
            }
            return options;
        }

        public static T WithMetadata<T>(this T options, IDictionary<string, object> metadata) where T : McpServerPrimitiveOptions => options.WithMetadata(metadata.AsEnumerable());

        public static T WithMetadata<T>(this T options, params KeyValuePair<string, object>[] metadata) where T : McpServerPrimitiveOptions => options.WithMetadata(metadata.AsEnumerable());
    }
}
