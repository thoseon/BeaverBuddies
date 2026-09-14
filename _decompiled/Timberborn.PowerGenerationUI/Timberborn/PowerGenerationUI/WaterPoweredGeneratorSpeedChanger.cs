using Timberborn.Debugging;
using Timberborn.QuickNotificationSystem;

namespace Timberborn.PowerGenerationUI;

internal class WaterPoweredGeneratorSpeedChanger : IDevModule
{
	private static readonly float SpeedChange = 0.1f;

	private readonly WaterPoweredGeneratorSpeedCalculator _waterPoweredGeneratorSpeedCalculator;

	private readonly QuickNotificationService _quickNotificationService;

	public WaterPoweredGeneratorSpeedChanger(WaterPoweredGeneratorSpeedCalculator waterPoweredGeneratorSpeedCalculator, QuickNotificationService quickNotificationService)
	{
		_waterPoweredGeneratorSpeedCalculator = waterPoweredGeneratorSpeedCalculator;
		_quickNotificationService = quickNotificationService;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Water wheels: increase max speed", IncreaseMaxSpeed)).AddMethod(DevMethod.Create("Water wheels: decrease max speed", DecreaseMaxSpeed)).Build();
	}

	private void IncreaseMaxSpeed()
	{
		_waterPoweredGeneratorSpeedCalculator.IncreaseMaxSpeed(SpeedChange);
		SendNotification();
	}

	private void DecreaseMaxSpeed()
	{
		_waterPoweredGeneratorSpeedCalculator.DecreaseMaxSpeed(SpeedChange);
		SendNotification();
	}

	private void SendNotification()
	{
		_quickNotificationService.SendNotification($"Water wheels max speed: {_waterPoweredGeneratorSpeedCalculator.MaxSpeed:F2}");
	}
}
