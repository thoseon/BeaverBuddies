using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.Healthcare;

internal record BeaverNeedShaderPropertySetterSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<BeaverNeedShaderPropertySet> PropertySets { get; init; }
}
