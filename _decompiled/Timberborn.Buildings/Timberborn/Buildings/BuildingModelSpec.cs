using Timberborn.BlueprintSystem;

namespace Timberborn.Buildings;

internal record BuildingModelSpec : ComponentSpec
{
	[Serialize]
	public string FinishedModelName { get; init; }

	[Serialize]
	public string UnfinishedModelName { get; init; }

	[Serialize]
	public string FinishedUncoveredModelName { get; init; }

	[Serialize]
	public string UndergroundModelName { get; init; }

	[Serialize]
	public ConstructionModeModel ConstructionModeModel { get; init; }

	[Serialize]
	public int UndergroundModelDepth { get; init; }

	public bool UnfinishedConstructionModeModel => ConstructionModeModel == ConstructionModeModel.Unfinished;
}
