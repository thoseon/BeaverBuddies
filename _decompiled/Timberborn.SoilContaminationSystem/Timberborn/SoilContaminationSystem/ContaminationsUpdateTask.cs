using System;
using Timberborn.Common;
using Timberborn.Multithreading;

namespace Timberborn.SoilContaminationSystem;

internal readonly struct ContaminationsUpdateTask(float[] contaminationLevels, bool[] contaminationsChangedLastTick, ReadOnlyArray<byte> terrainColumnCounts, ReadOnlyArray<float> contaminationCandidates, int xMapSize, int stride, int verticalStride, float contaminationThreshold, float contaminationPositiveEqualizationRate, float contaminationNegativeEqualizationRate) : IParallelizerLoopTask
{
	private readonly float[] _contaminationLevels = contaminationLevels;

	private readonly bool[] _contaminationsChangedLastTick = contaminationsChangedLastTick;

	private readonly ReadOnlyArray<byte> _terrainColumnCounts = terrainColumnCounts;

	private readonly ReadOnlyArray<float> _contaminationCandidates = contaminationCandidates;

	private readonly int _xMapSize = xMapSize;

	private readonly int _stride = stride;

	private readonly int _verticalStride = verticalStride;

	private readonly float _contaminationThreshold = contaminationThreshold;

	private readonly float _contaminationPositiveEqualizationRate = contaminationPositiveEqualizationRate;

	private readonly float _contaminationNegativeEqualizationRate = contaminationNegativeEqualizationRate;

	public void Run(int y)
	{
		int num = (y + 1) * _stride;
		for (int i = 0; i < _xMapSize; i++)
		{
			int num2 = i + 1 + num;
			byte b = _terrainColumnCounts[num2];
			for (int j = 0; j < b; j++)
			{
				int num3 = j * _verticalStride + num2;
				float num4 = _contaminationLevels[num3];
				float num5 = _contaminationCandidates[num3];
				float num6 = num5 - num4;
				float num7 = ((num6 > 0f) ? _contaminationPositiveEqualizationRate : _contaminationNegativeEqualizationRate);
				float num8 = ((num6 <= num7 && num6 >= 0f - num7) ? num5 : (num4 + (float)Math.Sign(num6) * num7));
				if (num8 < _contaminationThreshold)
				{
					num8 = 0f;
				}
				if (num8 != num4)
				{
					_contaminationLevels[num3] = num8;
					_contaminationsChangedLastTick[num3] = true;
				}
			}
		}
	}
}
