using Timberborn.BlueprintSystem;

namespace Timberborn.GoodStackSystem;

internal record GoodStackModelSpec : ComponentSpec
{
	[Serialize]
	public string LogObjectName { get; init; }

	[Serialize]
	public string BarrelObjectName { get; init; }

	[Serialize]
	public string BoxObjectName { get; init; }

	[Serialize]
	public string BagObjectName { get; init; }
}
