using Timberborn.BlueprintSystem;

namespace Timberborn.WorkshopsEffects;

internal record RecipeModel
{
	[Serialize]
	public string RecipeId { get; init; }

	[Serialize]
	public string ModelName { get; init; }
}
