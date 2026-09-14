using Timberborn.BlueprintSystem;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.WaterSystem;

public class WaterOverflowCalculator : ILoadableSingleton
{
	private readonly MapIndexService _mapIndexService;

	private readonly ISpecService _specService;

	private float _maxPressure;

	private float _overflowPressureFactorInverted;

	public WaterOverflowCalculator(MapIndexService mapIndexService, ISpecService specService)
	{
		_mapIndexService = mapIndexService;
		_specService = specService;
	}

	public void Load()
	{
		_maxPressure = _mapIndexService.TotalSize.z + 1;
		WaterSimulatorSpec singleSpec = _specService.GetSingleSpec<WaterSimulatorSpec>();
		_overflowPressureFactorInverted = 1f / singleSpec.OverflowPressureFactor;
	}

	public float ClampOverflow(float depth, int maxDepth, byte ceiling)
	{
		float num = depth - (float)maxDepth;
		float maxOverflow = GetMaxOverflow(ceiling);
		if (!(num > maxOverflow))
		{
			return num;
		}
		return maxOverflow;
	}

	public float GetOverflowSpace(float currentOverflow, byte ceiling)
	{
		return GetMaxOverflow(ceiling) - currentOverflow;
	}

	private float GetMaxOverflow(byte ceiling)
	{
		return (_maxPressure - (float)(int)ceiling) * _overflowPressureFactorInverted;
	}
}
