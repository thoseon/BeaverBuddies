using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.WaterSystem;

internal class WaterChangeService : ITickableSingleton, ILateTickable, IWaterRemovalService
{
	private readonly List<WaterChange> _threadSafeWaterChanges = new List<WaterChange>();

	private readonly List<WaterChange> _waterChanges = new List<WaterChange>();

	internal readonly Dictionary<Vector3Int, WaterAmountChange> RemovedWaterUnsafe = new Dictionary<Vector3Int, WaterAmountChange>();

	public ReadOnlyList<WaterChange> ThreadSafeWaterChanges => _threadSafeWaterChanges.AsReadOnlyList();

	public void Tick()
	{
		_threadSafeWaterChanges.Clear();
		_threadSafeWaterChanges.AddRange(_waterChanges);
		_waterChanges.Clear();
	}

	public void EnqueueWaterChange(Vector3Int coordinates, float depthChange, float contaminationChange)
	{
		_waterChanges.Add(new WaterChange(coordinates, depthChange, contaminationChange));
	}

	public WaterAmountChange GetWaterChangeUnsafe(Vector3Int coordinates)
	{
		if (!RemovedWaterUnsafe.TryGetValue(coordinates, out var value))
		{
			return new WaterAmountChange(0f, 0f);
		}
		return value;
	}
}
