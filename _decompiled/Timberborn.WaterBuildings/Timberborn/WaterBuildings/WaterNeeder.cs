using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.WaterBuildings;

internal class WaterNeeder : TickableComponent, IAwakableComponent, IFinishedStateListener, IWaterNeedingBuilding
{
	private BlockableObject _blockableObject;

	private WaterInput _waterInput;

	private IWaterInputCoordinates _waterInputCoordinates;

	private bool _isBlocked;

	public Vector3Int WaterCoordinatesTransformed => _waterInputCoordinates.Coordinates;

	public event EventHandler StartedNeedingWater;

	public event EventHandler StoppedNeedingWater;

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_waterInput = GetComponent<WaterInput>();
		_waterInputCoordinates = GetComponent<WaterInputPipeCoordinates>();
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public override void Tick()
	{
		if (_isBlocked && _waterInput.IsUnderwater)
		{
			UnblockBuilding();
		}
		else if (!_isBlocked && !_waterInput.IsUnderwater)
		{
			BlockBuilding();
		}
	}

	private void BlockBuilding()
	{
		_isBlocked = true;
		_blockableObject.Block(this);
		StartedNeedingWater?.Invoke(this, EventArgs.Empty);
	}

	private void UnblockBuilding()
	{
		_isBlocked = false;
		_blockableObject.Unblock(this);
		StoppedNeedingWater?.Invoke(this, EventArgs.Empty);
	}
}
