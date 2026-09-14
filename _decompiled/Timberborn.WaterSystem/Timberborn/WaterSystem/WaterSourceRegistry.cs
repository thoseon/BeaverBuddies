using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.MapEditorTickSystem;
using Timberborn.TickSystem;

namespace Timberborn.WaterSystem;

[MapEditorTickable]
internal class WaterSourceRegistry : ITickableSingleton
{
	private readonly List<IWaterSource> _waterSources = new List<IWaterSource>();

	private readonly List<ThreadSafeWaterSource> _threadSafeWaterSources = new List<ThreadSafeWaterSource>();

	public ReadOnlyList<ThreadSafeWaterSource> ThreadSafeWaterSources => _threadSafeWaterSources.AsReadOnlyList();

	public void Tick()
	{
		UpdateThreadSafeRegistry();
	}

	public void RegisterWaterSource(IWaterSource waterSource)
	{
		_waterSources.Add(waterSource);
	}

	public void UnregisterWaterSource(IWaterSource waterSource)
	{
		_waterSources.Remove(waterSource);
	}

	private void UpdateThreadSafeRegistry()
	{
		_threadSafeWaterSources.Clear();
		foreach (IWaterSource waterSource in _waterSources)
		{
			_threadSafeWaterSources.Add(new ThreadSafeWaterSource(waterSource));
		}
	}
}
