using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.TemplateAttachmentSystem;

internal record TemplateAttachmentsSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<AttachmentDefinition> Attachments { get; init; }
}
