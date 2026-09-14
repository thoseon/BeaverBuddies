using Timberborn.BlueprintSystem;

namespace Timberborn.Demolishing;

internal record DemolishableSpec : ComponentSpec
{
	[Serialize]
	public float DemolishTimeInHours { get; init; }

	[Serialize]
	public bool ShowDemolishButtonInEntityPanel { get; init; }
}
