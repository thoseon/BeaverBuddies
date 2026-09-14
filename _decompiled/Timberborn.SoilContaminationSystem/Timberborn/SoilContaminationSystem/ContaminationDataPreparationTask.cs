using System;
using Timberborn.Multithreading;

namespace Timberborn.SoilContaminationSystem;

internal readonly struct ContaminationDataPreparationTask(float[] contaminationCandidates, float[] lastTickContaminationCandidates, bool[] contaminationsChangedLastTick) : IParallelizerSingleTask
{
	private readonly float[] _contaminationCandidates = contaminationCandidates;

	private readonly float[] _lastTickContaminationCandidates = lastTickContaminationCandidates;

	private readonly bool[] _contaminationsChangedLastTick = contaminationsChangedLastTick;

	public void Run()
	{
		MemoryExtensions.AsSpan(_contaminationCandidates).CopyTo(_lastTickContaminationCandidates);
		Array.Clear(_contaminationsChangedLastTick, 0, _contaminationsChangedLastTick.Length);
	}
}
