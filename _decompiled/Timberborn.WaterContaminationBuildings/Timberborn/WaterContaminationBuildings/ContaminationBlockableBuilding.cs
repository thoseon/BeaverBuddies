using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.EntitySystem;
using Timberborn.TickSystem;
using Timberborn.WaterBuildings;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WaterContaminationBuildings;

public class ContaminationBlockableBuilding : TickableComponent, IAwakableComponent, IPostLoadableEntity, IFinishedStateListener
{
	private static readonly float MaximumWaterContamination = 0.05f;

	private static readonly float Offset = 0.005f;

	private static readonly float ContaminationToUnblock = MaximumWaterContamination - Offset;

	private static readonly float ContaminationToBlock = MaximumWaterContamination + Offset;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private BlockableObject _blockableObject;

	private IWaterNeedingBuilding _waterNeedingBuilding;

	public bool IsBlocked { get; private set; }

	public event EventHandler BlockedByContamination;

	public event EventHandler UnblockedByContamination;

	public ContaminationBlockableBuilding(IThreadSafeWaterMap threadSafeWaterMap)
	{
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_waterNeedingBuilding = GetComponent<IWaterNeedingBuilding>();
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		CheckContamination();
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
	}

	public void PostLoadEntity()
	{
		if (base.Enabled)
		{
			CheckContamination();
		}
	}

	public override void Tick()
	{
		CheckContamination();
	}

	private void CheckContamination()
	{
		Vector3Int waterCoordinatesTransformed = _waterNeedingBuilding.WaterCoordinatesTransformed;
		float num = _threadSafeWaterMap.ColumnContamination(waterCoordinatesTransformed);
		if (num <= ContaminationToUnblock && IsBlocked)
		{
			UnblockBuilding();
		}
		else if (num > ContaminationToBlock && !IsBlocked)
		{
			BlockBuilding();
		}
	}

	private void BlockBuilding()
	{
		IsBlocked = true;
		_blockableObject.Block(this);
		BlockedByContamination?.Invoke(this, EventArgs.Empty);
	}

	private void UnblockBuilding()
	{
		IsBlocked = false;
		_blockableObject.Unblock(this);
		UnblockedByContamination?.Invoke(this, EventArgs.Empty);
	}
}
