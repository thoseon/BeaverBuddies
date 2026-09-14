using Timberborn.Common;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.SlotSystem;

public class PatrollingSlotFactory
{
	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	public PatrollingSlotFactory(IRandomNumberGenerator randomNumberGenerator, IThreadSafeWaterMap threadSafeWaterMap)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public PatrollingSlot Create(Transform slotTransform, Transform start, Transform end, PatrollingSlotSpec patrollingSlotSpec)
	{
		return new PatrollingSlot(_randomNumberGenerator, slotTransform, start, end, patrollingSlotSpec, _threadSafeWaterMap);
	}
}
