using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.WaterSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.WaterBuildings;

public class WaterInput : BaseComponent, IPersistentEntity, IAwakableComponent, IFinishedStateListener
{
	private static readonly int BufferMultiplier = 3;

	private static readonly ComponentKey WaterInputKey = new ComponentKey("WaterInput");

	private static readonly PropertyKey<float> CleanWaterAmountKey = new PropertyKey<float>("CleanWaterAmount");

	private static readonly PropertyKey<float> ContaminatedWaterAmountKey = new PropertyKey<float>("ContaminatedWaterAmount");

	private readonly IWaterService _waterService;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly WaterInputService _waterInputService;

	private IWaterInputCoordinates _inputCoordinates;

	private float _contaminatedWaterAmount;

	private float _cleanWaterAmount;

	public bool IsUnderwater
	{
		get
		{
			if (!_inputCoordinates.IsBlocked)
			{
				return _threadSafeWaterMap.CellIsUnderwater(_inputCoordinates.Coordinates);
			}
			return false;
		}
	}

	public float ContaminationPercentage => _threadSafeWaterMap.ColumnContamination(_inputCoordinates.Coordinates);

	public Vector3Int Coordinates => _inputCoordinates.Coordinates;

	public WaterInput(IWaterService waterService, IThreadSafeWaterMap threadSafeWaterMap, WaterInputService waterInputService)
	{
		_waterService = waterService;
		_threadSafeWaterMap = threadSafeWaterMap;
		_waterInputService = waterInputService;
	}

	public void Awake()
	{
		_inputCoordinates = GetComponent<IWaterInputCoordinates>();
	}

	public void OnEnterFinishedState()
	{
		_waterInputService.RegisterWaterInput(this);
	}

	public void OnExitFinishedState()
	{
		_waterInputService.UnregisterWaterInput(this);
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(WaterInputKey);
		component.Set(CleanWaterAmountKey, _cleanWaterAmount);
		component.Set(ContaminatedWaterAmountKey, _contaminatedWaterAmount);
	}

	[BackwardCompatible(2026, 4, 13, Compatibility.Save)]
	public void Load(IEntityLoader entityLoader)
	{
		if (entityLoader.TryGetComponent(WaterInputKey, out var objectLoader))
		{
			_cleanWaterAmount = objectLoader.Get(CleanWaterAmountKey);
			_contaminatedWaterAmount = objectLoader.Get(ContaminatedWaterAmountKey);
		}
	}

	public void AddWater(float cleanWaterAmount, float contaminatedWaterAmount)
	{
		_cleanWaterAmount += cleanWaterAmount;
		_contaminatedWaterAmount += contaminatedWaterAmount;
	}

	public void RemoveCleanWater(float waterAmount)
	{
		_cleanWaterAmount -= waterAmount;
	}

	public void RemoveContaminatedWater(float waterAmount)
	{
		_contaminatedWaterAmount -= waterAmount;
	}

	public float DemandCleanWaterAmount(float neededWater)
	{
		if (neededWater * (float)BufferMultiplier > _cleanWaterAmount)
		{
			_waterService.RemoveCleanWater(_inputCoordinates.Coordinates, neededWater);
		}
		if (!(_cleanWaterAmount <= 0f))
		{
			return _cleanWaterAmount;
		}
		return 0f;
	}

	public float DemandContaminatedWaterAmount(float neededWater)
	{
		if (neededWater * (float)BufferMultiplier > _contaminatedWaterAmount)
		{
			_waterService.RemoveContaminatedWater(_inputCoordinates.Coordinates, neededWater);
		}
		if (!(_contaminatedWaterAmount <= 0f))
		{
			return _contaminatedWaterAmount;
		}
		return 0f;
	}
}
