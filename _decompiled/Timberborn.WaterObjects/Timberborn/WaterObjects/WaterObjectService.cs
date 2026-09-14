using System.Collections.Generic;
using Timberborn.MapEditorTickSystem;
using Timberborn.TickSystem;

namespace Timberborn.WaterObjects;

[MapEditorTickable]
public class WaterObjectService : ITickableSingleton
{
	private readonly List<WaterObject> _waterObjects = new List<WaterObject>();

	public void RegisterWaterObject(WaterObject waterObject)
	{
		_waterObjects.Add(waterObject);
	}

	public void UnregisterWaterObject(WaterObject waterObject)
	{
		_waterObjects.Remove(waterObject);
	}

	public void Tick()
	{
		foreach (WaterObject waterObject in _waterObjects)
		{
			waterObject.UpdateWaterAboveBase();
		}
	}
}
