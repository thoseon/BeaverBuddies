using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BlockSystem;
using Timberborn.Coordinates;
using Timberborn.EnterableSystem;
using Timberborn.NaturalResources;
using Timberborn.Persistence;
using Timberborn.TemplateSystem;
using Timberborn.TimeSystem;
using Timberborn.WorkSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Planting;

public class PlantExecutor : BaseComponent, IAwakableComponent, IExecutor
{
	private static readonly ComponentKey PlantExecutorKey = new ComponentKey("PlantExecutor");

	private static readonly PropertyKey<string> NaturalResourceKey = new PropertyKey<string>("NaturalResource");

	private static readonly PropertyKey<Vector3Int> CoordinatesKey = new PropertyKey<Vector3Int>("Coordinates");

	private static readonly PropertyKey<float> FinishTimestampKey = new PropertyKey<float>("FinishTimestamp");

	private readonly IDayNightCycle _dayNightCycle;

	private readonly TemplateNameMapper _templateNameMapper;

	private readonly NaturalResourceFactory _naturalResourceFactory;

	private readonly PlantingService _plantingService;

	private readonly BlockValidator _blockValidator;

	private Enterer _enterer;

	private Worker _worker;

	private float _finishTimestamp;

	private Vector3Int _coordinates;

	private string _naturalResource;

	public bool IsPlanting { get; private set; }

	public event EventHandler PlantingStarted;

	public event EventHandler PlantingFinished;

	public PlantExecutor(IDayNightCycle dayNightCycle, TemplateNameMapper templateNameMapper, NaturalResourceFactory naturalResourceFactory, PlantingService plantingService, BlockValidator blockValidator)
	{
		_dayNightCycle = dayNightCycle;
		_templateNameMapper = templateNameMapper;
		_naturalResourceFactory = naturalResourceFactory;
		_plantingService = plantingService;
		_blockValidator = blockValidator;
	}

	public void Awake()
	{
		_enterer = GetComponent<Enterer>();
		_worker = GetComponent<Worker>();
	}

	public bool Launch(Vector3Int coordinates, string resource)
	{
		if (_enterer.IsInside || !_worker.Workplace || string.IsNullOrEmpty(resource))
		{
			return false;
		}
		_coordinates = coordinates;
		_naturalResource = resource;
		float hours = _templateNameMapper.GetTemplate(_naturalResource).GetSpec<PlantableSpec>().PlantTimeInHours / _worker.WorkingSpeedMultiplier;
		_finishTimestamp = _dayNightCycle.DayNumberHoursFromNow(hours);
		StartPlanting();
		return true;
	}

	public ExecutorStatus Tick(float deltaTimeInHours)
	{
		if (!_worker.Workplace || _naturalResource == null || _plantingService.GetResourceAt(_coordinates) != _naturalResource)
		{
			FinishPlanting();
			return ExecutorStatus.Failure;
		}
		if (_dayNightCycle.PartialDayNumber > _finishTimestamp)
		{
			SpawnResource();
			FinishPlanting();
			return ExecutorStatus.Success;
		}
		return ExecutorStatus.Running;
	}

	public void Save(IEntitySaver entitySaver)
	{
		IObjectSaver component = entitySaver.GetComponent(PlantExecutorKey);
		component.Set(CoordinatesKey, _coordinates);
		component.Set(NaturalResourceKey, _naturalResource);
		component.Set(FinishTimestampKey, _finishTimestamp);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(PlantExecutorKey);
		_coordinates = component.Get(CoordinatesKey);
		_naturalResource = component.Get(NaturalResourceKey);
		_finishTimestamp = component.Get(FinishTimestampKey);
		StartPlanting();
	}

	private void SpawnResource()
	{
		BlockObjectSpec spec = _templateNameMapper.GetTemplate(_naturalResource).GetSpec<BlockObjectSpec>();
		if (_blockValidator.BlocksValid(spec, new Placement(_coordinates)))
		{
			_naturalResourceFactory.PlantNew(_naturalResource, _coordinates);
		}
	}

	private void StartPlanting()
	{
		_plantingService.ReservePlantingCoordinates(_coordinates);
		PlantingStarted?.Invoke(this, EventArgs.Empty);
		IsPlanting = true;
	}

	private void FinishPlanting()
	{
		_plantingService.UnreservePlantingCoordinates(_coordinates);
		PlantingFinished?.Invoke(this, EventArgs.Empty);
		IsPlanting = false;
	}
}
