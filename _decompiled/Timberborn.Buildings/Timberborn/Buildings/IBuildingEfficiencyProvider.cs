namespace Timberborn.Buildings;

public interface IBuildingEfficiencyProvider
{
	bool CanUse { get; }

	float Efficiency { get; }
}
