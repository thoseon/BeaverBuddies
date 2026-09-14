using System;
using Timberborn.BlueprintSystem;
using Timberborn.Localization;
using Timberborn.TimeSystem;
using Timberborn.TutorialSystem;

namespace Timberborn.TutorialSteps;

internal class GameSpeedStepDeserializer : IStepDeserializer
{
	private readonly SpeedManager _speedManager;

	private readonly ILoc _loc;

	public GameSpeedStepDeserializer(SpeedManager speedManager, ILoc loc)
	{
		_speedManager = speedManager;
		_loc = loc;
	}

	public bool TryDeserialize(Blueprint step, out TutorialStep tutorialStep)
	{
		if (step.Specs[0] is GameSpeedStepSpec gameSpeedStepSpec)
		{
			tutorialStep = Create(gameSpeedStepSpec.Speed, gameSpeedStepSpec.OnlyOnce);
			return true;
		}
		tutorialStep = null;
		return false;
	}

	private TutorialStep Create(int speed, bool onlyOnce)
	{
		return TutorialStep.Create(new GameSpeedStep(_speedManager, GetDescription(speed), GetActualSpeed(speed), onlyOnce), GetKeyBindingKey(speed));
	}

	private string GetDescription(int speed)
	{
		return _loc.T(GetLocKey(speed));
	}

	private static string GetLocKey(int speed)
	{
		return speed switch
		{
			1 => "Tutorial.Basics.SetSpeed1", 
			2 => "Tutorial.Basics.SetSpeed2", 
			3 => "Tutorial.Basics.SetSpeed3", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private static int GetActualSpeed(int speed)
	{
		return speed switch
		{
			1 => 1, 
			2 => 3, 
			3 => 7, 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}

	private static string GetKeyBindingKey(int speed)
	{
		return speed switch
		{
			1 => "Speed1", 
			2 => "Speed2", 
			3 => "Speed3", 
			_ => throw new ArgumentOutOfRangeException(), 
		};
	}
}
