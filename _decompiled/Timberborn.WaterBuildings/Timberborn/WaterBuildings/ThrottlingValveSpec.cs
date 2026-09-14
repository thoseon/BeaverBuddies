using Timberborn.BlueprintSystem;

namespace Timberborn.WaterBuildings;

internal record ThrottlingValveSpec : ComponentSpec
{
	[Serialize]
	public float MaxOutflowLimit { get; init; }

	[Serialize]
	public float OutflowLimitStep { get; init; }

	[Serialize]
	public bool DefaultOutflowLimitEnabled { get; init; }

	[Serialize]
	public float DefaultOutflowLimit { get; init; }

	[Serialize]
	public bool DefaultAutomationOutflowLimitEnabled { get; init; }

	[Serialize]
	public float DefaultAutomationOutflowLimit { get; init; }

	[Serialize]
	public float RateOfChangeHighPrimary { get; init; }

	[Serialize]
	public float RateOfChangeHighSecondary { get; init; }

	[Serialize]
	public float RateOfChangeLowPrimary { get; init; }

	[Serialize]
	public float RateOfChangeLowSecondary { get; init; }

	[Serialize]
	public int RateOfChangePrimaryTicks { get; init; }

	[Serialize]
	public int RateOfChangePrimaryToSecondaryTicks { get; init; }

	[Serialize]
	public float ReactionSpeedExponent { get; init; }

	[Serialize]
	public float ReactionSpeedStep { get; init; }
}
