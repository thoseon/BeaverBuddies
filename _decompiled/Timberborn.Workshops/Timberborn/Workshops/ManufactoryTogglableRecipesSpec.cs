using Timberborn.BlueprintSystem;

namespace Timberborn.Workshops;

internal record ManufactoryTogglableRecipesSpec : ComponentSpec
{
	[Serialize]
	public string LabelLocKey { get; init; }
}
