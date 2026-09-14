using System;
using Timberborn.Multithreading;

namespace Timberborn.SoilMoistureSystem;

internal readonly struct MoistureDataPreparationTask(float[] moistureLevels, float[] lastTickMoistureLevels, bool[] moistureLevelsChangedLastTick) : IParallelizerSingleTask
{
	private readonly float[] _moistureLevels = moistureLevels;

	private readonly float[] _lastTickMoistureLevels = lastTickMoistureLevels;

	private readonly bool[] _moistureLevelsChangedLastTick = moistureLevelsChangedLastTick;

	public void Run()
	{
		MemoryExtensions.AsSpan(_moistureLevels).CopyTo(_lastTickMoistureLevels);
		Array.Clear(_moistureLevelsChangedLastTick, 0, _moistureLevelsChangedLastTick.Length);
	}
}
