using System;
using Timberborn.TimeSystem;

namespace Timberborn.PowerGenerationUI;

internal class WaterPoweredGeneratorSpeedCalculator
{
	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	public float MaxSpeed { get; private set; } = 1f;

	public WaterPoweredGeneratorSpeedCalculator(NonlinearAnimationManager nonlinearAnimationManager)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
	}

	public float CalculateSpeed(float generatedRotation)
	{
		int num = ((!(generatedRotation < 0f)) ? 1 : (-1));
		return Math.Min(Math.Abs(generatedRotation), MaxSpeed) * _nonlinearAnimationManager.SpeedMultiplier * (float)num;
	}

	public void IncreaseMaxSpeed(float change)
	{
		MaxSpeed += change;
	}

	public void DecreaseMaxSpeed(float change)
	{
		MaxSpeed -= change;
	}
}
