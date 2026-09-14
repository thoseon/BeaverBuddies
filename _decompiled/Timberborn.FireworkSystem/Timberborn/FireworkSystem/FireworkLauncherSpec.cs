using Timberborn.BlueprintSystem;

namespace Timberborn.FireworkSystem;

internal record FireworkLauncherSpec : ComponentSpec
{
	[Serialize]
	public string Turret { get; init; }

	[Serialize]
	public string Barrel { get; init; }

	[Serialize]
	public string GoodId { get; init; }

	[Serialize]
	public int GoodAmount { get; init; }
}
