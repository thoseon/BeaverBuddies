using Timberborn.Common;
using Timberborn.Multithreading;

namespace Timberborn.SoilMoistureSystem;

internal readonly struct WaterEvaporationCalculationTask(float[] evaporationModifiers, in ReadOnlyArray<byte> columnCounts, in ReadOnlyArray<byte> clusterSaturations, in ReadOnlyArray<float> saturationToEvaporationMap, int xMapSize, int stride, int verticalStride) : IParallelizerLoopTask
{
	private readonly float[] _evaporationModifiers = evaporationModifiers;

	private readonly ReadOnlyArray<byte> _columnCounts = columnCounts;

	private readonly ReadOnlyArray<byte> _clusterSaturations = clusterSaturations;

	private readonly ReadOnlyArray<float> _saturationToEvaporationMap = saturationToEvaporationMap;

	private readonly int _xMapSize = xMapSize;

	private readonly int _stride = stride;

	private readonly int _verticalStride = verticalStride;

	public void Run(int y)
	{
		int num = (y + 1) * _stride;
		for (int i = 0; i < _xMapSize; i++)
		{
			int num2 = i + 1 + num;
			byte b = _columnCounts[num2];
			for (int j = 0; j < b; j++)
			{
				int num3 = num2 + j * _verticalStride;
				byte b2 = _clusterSaturations[num3];
				float num4 = ((b2 == 0) ? 1f : _saturationToEvaporationMap[b2]);
				_evaporationModifiers[num3] = num4;
			}
		}
	}
}
