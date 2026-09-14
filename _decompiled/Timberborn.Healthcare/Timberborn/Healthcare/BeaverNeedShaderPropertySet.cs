using Timberborn.BlueprintSystem;

namespace Timberborn.Healthcare;

internal record BeaverNeedShaderPropertySet
{
	[Serialize]
	public string NeedId { get; init; }

	[Serialize]
	public string PropertyName { get; init; }
}
