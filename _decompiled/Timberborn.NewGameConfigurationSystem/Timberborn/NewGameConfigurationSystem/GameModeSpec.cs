using Timberborn.BlueprintSystem;

namespace Timberborn.NewGameConfigurationSystem;

public record GameModeSpec : ComponentSpec
{
	[Serialize]
	public int Order { get; init; }

	[Serialize]
	public bool IsDefault { get; init; }

	[Serialize]
	public string DisplayNameLocKey { get; init; }

	[Serialize]
	public string DescriptionLocKey { get; init; }

	[Serialize]
	public int StartingAdults { get; init; }

	[Serialize]
	public MinMaxSpec<float> AdultAgeProgress { get; init; }

	[Serialize]
	public int StartingChildren { get; init; }

	[Serialize]
	public MinMaxSpec<float> ChildAgeProgress { get; init; }

	[Serialize]
	public float FoodConsumption { get; init; }

	[Serialize]
	public float WaterConsumption { get; init; }

	[Serialize]
	public int StartingFood { get; init; }

	[Serialize]
	public int StartingWater { get; init; }

	[Serialize]
	public MinMaxSpec<int> TemperateWeatherDuration { get; init; }

	[Serialize]
	public MinMaxSpec<int> DroughtDuration { get; init; }

	[Serialize]
	public float DroughtDurationHandicapMultiplier { get; init; }

	[Serialize]
	public int DroughtDurationHandicapCycles { get; init; }

	[Serialize]
	public int CyclesBeforeRandomizingBadtide { get; init; }

	[Serialize]
	public float ChanceForBadtide { get; init; }

	[Serialize]
	public MinMaxSpec<int> BadtideDuration { get; init; }

	[Serialize]
	public float BadtideDurationHandicapMultiplier { get; init; }

	[Serialize]
	public int BadtideDurationHandicapCycles { get; init; }

	[Serialize]
	public float InjuryChance { get; init; }

	[Serialize]
	public float DemolishableRecoveryRate { get; init; }

	public override string ToString()
	{
		return string.Format("{0}: {1}\n", "Order", Order) + "DisplayNameLocKey: " + DisplayNameLocKey + "\nDescriptionLocKey: " + DescriptionLocKey + "\n" + string.Format("{0}: {1}\n", "StartingAdults", StartingAdults) + string.Format("{0}: {1}\n", "AdultAgeProgress", AdultAgeProgress) + string.Format("{0}: {1}\n", "StartingChildren", StartingChildren) + string.Format("{0}: {1}\n", "ChildAgeProgress", ChildAgeProgress) + string.Format("{0}: {1}\n", "FoodConsumption", FoodConsumption) + string.Format("{0}: {1}\n", "WaterConsumption", WaterConsumption) + string.Format("{0}: {1}\n", "StartingFood", StartingFood) + string.Format("{0}: {1}\n", "StartingWater", StartingWater) + string.Format("{0}: {1}\n", "TemperateWeatherDuration", TemperateWeatherDuration) + string.Format("{0}: {1}\n", "DroughtDuration", DroughtDuration) + string.Format("{0}: {1}\n", "DroughtDurationHandicapMultiplier", DroughtDurationHandicapMultiplier) + string.Format("{0}: {1}\n", "DroughtDurationHandicapCycles", DroughtDurationHandicapCycles) + string.Format("{0}: {1}\n", "CyclesBeforeRandomizingBadtide", CyclesBeforeRandomizingBadtide) + string.Format("{0}: {1}\n", "ChanceForBadtide", ChanceForBadtide) + string.Format("{0}: {1}\n", "BadtideDuration", BadtideDuration) + string.Format("{0}: {1}\n", "BadtideDurationHandicapMultiplier", BadtideDurationHandicapMultiplier) + string.Format("{0}: {1}\n", "BadtideDurationHandicapCycles", BadtideDurationHandicapCycles) + string.Format("{0}: {1}\n", "InjuryChance", InjuryChance) + string.Format("{0}: {1}", "DemolishableRecoveryRate", DemolishableRecoveryRate);
	}
}
