using System.Collections.Generic;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;

namespace Timberborn.WaterBuildings;

public class WaterInputService : ITickableSingleton
{
	private readonly IWaterRemovalService _waterRemovalService;

	private readonly List<WaterInput> _waterInputs = new List<WaterInput>();

	public WaterInputService(IWaterRemovalService waterRemovalService)
	{
		_waterRemovalService = waterRemovalService;
	}

	public void RegisterWaterInput(WaterInput waterInput)
	{
		_waterInputs.Add(waterInput);
	}

	public void UnregisterWaterInput(WaterInput waterInput)
	{
		_waterInputs.Remove(waterInput);
	}

	public void Tick()
	{
		foreach (WaterInput waterInput in _waterInputs)
		{
			WaterAmountChange waterChangeUnsafe = _waterRemovalService.GetWaterChangeUnsafe(waterInput.Coordinates);
			waterInput.AddWater(waterChangeUnsafe.CleanWaterChange, waterChangeUnsafe.ContaminatedWaterChange);
		}
	}
}
