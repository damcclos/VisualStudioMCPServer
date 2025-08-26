using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace McpService
{
    public record struct AudienceRole(string Name, Uri Uri, IReadOnlyDictionary<string, object> Metadata);
    public record struct ResourceAnnotation(IReadOnlyList<AudienceRole> Audience, DateTime LastModified, double Priority);

    public interface IMcpServerResource : IMcpServerPrimitive
    {
        Uri Uri { get; }
        string MimeType { get; }
        ulong Size { get; }

        IReadOnlyList<ResourceAnnotation> Annotations { get; }
    }

    public abstract class McpServerResource : McpServerPrimitive, IMcpServerResource
    {
        public Uri Uri { get; }
        public string MimeType { get; }
        public ulong Size { get; }
        public IReadOnlyList<ResourceAnnotation> Annotations { get; }

        protected McpServerResource(string name, Uri uri, string? title = null, string? description = null, string? mimeType = null, ulong? size = null, IEnumerable<ResourceAnnotation>? annotations = null, IEnumerable<KeyValuePair<string, object>>? metadata = null)
            : base(name, title, description, metadata)
        {
            Uri = uri;
            MimeType = mimeType ?? string.Empty;
            Size = size ?? 0;
            Annotations = annotations?.ToImmutableList() ?? ImmutableList<ResourceAnnotation>.Empty;
        }

        protected McpServerResource(McpServerResourceOptions options)
            : this(options.Name, options.Uri, options.Title, options.Description, options.MimeType, options.Size, options.Annotations, options.Metadata)
        {}
    }
}
