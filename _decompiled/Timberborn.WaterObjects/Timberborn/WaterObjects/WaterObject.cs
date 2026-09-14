using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WaterObjects;

public class WaterObject : BaseComponent, IAwakableComponent, IFinishedPostLoadStateListener
{
	private readonly WaterObjectService _waterObjectService;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private BlockObject _blockObject;

	private IWaterObjectSpecification _specification;

	private Vector3Int _baseCoordinates;

	public int WaterAboveBase { get; private set; }

	public event EventHandler WaterAboveBaseChanged;

	public WaterObject(WaterObjectService waterObjectService, IThreadSafeWaterMap threadSafeWaterMap)
	{
		_waterObjectService = waterObjectService;
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_specification = GetComponent<IWaterObjectSpecification>();
	}

	public void OnEnterFinishedPostLoadState()
	{
		_baseCoordinates = GetBaseCoordinates();
		_waterObjectService.RegisterWaterObject(this);
		UpdateWaterAboveBase(CurrentWaterAboveBase(_baseCoordinates));
	}

	public void OnExitFinishedPostLoadState()
	{
		_waterObjectService.UnregisterWaterObject(this);
	}

	public void UpdateWaterAboveBase()
	{
		int num = CurrentWaterAboveBase(_baseCoordinates);
		if (num != WaterAboveBase)
		{
			UpdateWaterAboveBase(num);
		}
	}

	public bool IsPreviewUnderWater()
	{
		return CurrentWaterAboveBase(GetBaseCoordinates()) > 0;
	}

	private Vector3Int GetBaseCoordinates()
	{
		return _blockObject.TransformCoordinates(_specification.WaterCoordinates) + new Vector3Int(0, 0, _blockObject.BaseZ);
	}

	private int CurrentWaterAboveBase(Vector3Int coordinatesToCheck)
	{
		int b = _threadSafeWaterMap.CeiledWaterHeight(coordinatesToCheck) - coordinatesToCheck.z;
		return Mathf.Max(0, b);
	}

	private void UpdateWaterAboveBase(int currentWaterAboveBase)
	{
		WaterAboveBase = currentWaterAboveBase;
		WaterAboveBaseChanged?.Invoke(this, EventArgs.Empty);
	}
}
