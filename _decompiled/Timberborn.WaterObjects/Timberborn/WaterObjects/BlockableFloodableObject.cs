using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;

namespace Timberborn.WaterObjects;

public class BlockableFloodableObject : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private FloodableObject _floodableObject;

	private BlockableObject _blockableObject;

	public void Awake()
	{
		_floodableObject = GetComponent<FloodableObject>();
		_blockableObject = GetComponent<BlockableObject>();
	}

	public void OnEnterFinishedState()
	{
		if (_floodableObject.IsFlooded)
		{
			BlockBuilding();
		}
		_floodableObject.Flooded += OnFlooded;
		_floodableObject.Unflooded += OnUnflooded;
	}

	public void OnExitFinishedState()
	{
		_floodableObject.Flooded -= OnFlooded;
		_floodableObject.Unflooded -= OnUnflooded;
	}

	private void OnFlooded(object sender, EventArgs e)
	{
		BlockBuilding();
	}

	private void OnUnflooded(object sender, EventArgs e)
	{
		UnblockBuilding();
	}

	private void BlockBuilding()
	{
		_blockableObject.Block(this);
	}

	private void UnblockBuilding()
	{
		_blockableObject.Unblock(this);
	}
}
