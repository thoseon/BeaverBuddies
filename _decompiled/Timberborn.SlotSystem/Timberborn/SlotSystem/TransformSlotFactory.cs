using Timberborn.Common;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.SlotSystem;

public class TransformSlotFactory
{
	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	public TransformSlotFactory(IRandomNumberGenerator randomNumberGenerator, IThreadSafeWaterMap threadSafeWaterMap)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public TransformSlot Create(Transform followedTransform, TransformSlotSpec transformSlotSpec)
	{
		return new TransformSlot(_randomNumberGenerator, _threadSafeWaterMap, followedTransform, transformSlotSpec);
	}
}
