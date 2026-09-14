using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Demolishing;

internal record DemolishableParticle
{
	[Serialize]
	public string AttachmentId { get; init; }

	[Serialize]
	public ImmutableArray<string> TemplateNames { get; init; }
}
