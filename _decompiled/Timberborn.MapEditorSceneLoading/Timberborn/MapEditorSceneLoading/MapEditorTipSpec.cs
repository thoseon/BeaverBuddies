using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.MapEditorSceneLoading;

internal record MapEditorTipSpec : ComponentSpec
{
	[Serialize]
	public ImmutableArray<string> Tips { get; init; }
}
