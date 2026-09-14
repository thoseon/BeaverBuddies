namespace Timberborn.Workshops;

public interface IManufactoryLimiter
{
	float ProductionEfficiency();

	float MaxProductionProgressChange(float expectedProductionProgressChange);
}
