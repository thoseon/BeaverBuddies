using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.WaterObjects;

namespace Timberborn.WaterBuildings;

internal class FloodableFire : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private Fire _fire;

	private FloodableObject _floodableObject;

	public void Awake()
	{
		_fire = GetComponent<Fire>();
		_floodableObject = GetComponent<FloodableObject>();
	}

	public void OnEnterFinishedState()
	{
		_floodableObject.Flooded += OnFlooded;
		_floodableObject.Unflooded += OnUnflooded;
		if (!_floodableObject.IsFlooded)
		{
			_fire.Enable();
		}
	}

	public void OnExitFinishedState()
	{
		_floodableObject.Flooded -= OnFlooded;
		_floodableObject.Unflooded -= OnUnflooded;
		_fire.Disable();
	}

	private void OnFlooded(object sender, EventArgs e)
	{
		_fire.Disable();
	}

	private void OnUnflooded(object sender, EventArgs e)
	{
		_fire.Enable();
	}
}
