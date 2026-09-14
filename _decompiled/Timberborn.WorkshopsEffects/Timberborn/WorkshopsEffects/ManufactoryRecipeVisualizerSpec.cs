using System.Collections.Immutable;
using Timberborn.BlueprintSystem;

namespace Timberborn.WorkshopsEffects;

internal record ManufactoryRecipeVisualizerSpec : ComponentSpec
{
	[Serialize]
	public string InitialModelName { get; init; }

	[Serialize]
	public ImmutableArray<RecipeModel> RecipeModels { get; init; }
}
