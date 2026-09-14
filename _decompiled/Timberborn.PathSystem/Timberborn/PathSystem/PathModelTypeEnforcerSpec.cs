using Timberborn.BlueprintSystem;

namespace Timberborn.PathSystem;

internal record PathModelTypeEnforcerSpec : ComponentSpec
{
	[Serialize]
	public PathModelType PathModelType { get; init; }
}
