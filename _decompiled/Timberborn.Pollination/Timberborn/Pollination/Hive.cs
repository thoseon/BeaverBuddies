using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;
using Timberborn.BuildingRange;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.TemplateSystem;
using Timberborn.TimeSystem;
using Timberborn.WorldPersistence;
using UnityEngine;

namespace Timberborn.Pollination;

public class Hive : BaseComponent, IAwakableComponent, IFinishedStateListener, IBuildingWithRange, IPersistentEntity
{
	private static readonly ComponentKey HiveKey = new ComponentKey("Hive");

	private static readonly PropertyKey<float> PollinationProgressKey = new PropertyKey<float>("PollinationProgress");

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private readonly IBlockService _blockService;

	private readonly ITimeTriggerFactory _timeTriggerFactory;

	private BlockObjectRange _blockObjectRange;

	private TemplateSpec _templateSpec;

	private BlockableObject _blockableObject;

	private HiveSpec _hiveSpec;

	private ITimeTrigger _timeTrigger;

	private readonly List<Pollinatee> _nearbyPollinatees = new List<Pollinatee>();

	public string RangeName => _templateSpec.TemplateName;

	public Hive(IRandomNumberGenerator randomNumberGenerator, IBlockService blockService, ITimeTriggerFactory timeTriggerFactory)
	{
		_randomNumberGenerator = randomNumberGenerator;
		_blockService = blockService;
		_timeTriggerFactory = timeTriggerFactory;
	}

	public void Awake()
	{
		_blockObjectRange = GetComponent<BlockObjectRange>();
		_templateSpec = GetComponent<TemplateSpec>();
		_blockableObject = GetComponent<BlockableObject>();
		_hiveSpec = GetComponent<HiveSpec>();
		_timeTrigger = _timeTriggerFactory.Create(PollinateNearbyPollinatees, _hiveSpec.HoursBetweenPollinations / 24f);
		DisableComponent();
	}

	public IEnumerable<BaseComponent> GetObjectsInRange()
	{
		foreach (Vector3Int item in GetBlocksInRange())
		{
			Pollinatee bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<Pollinatee>(item);
			if (bottomObjectComponentAt != null)
			{
				yield return bottomObjectComponentAt;
			}
		}
	}

	public void OnEnterFinishedState()
	{
		_blockableObject.ObjectBlocked += OnObjectBlocked;
		_blockableObject.ObjectUnblocked += OnObjectUnblocked;
		if (_blockableObject.IsUnblocked)
		{
			_timeTrigger.Resume();
		}
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		_blockableObject.ObjectBlocked -= OnObjectBlocked;
		_blockableObject.ObjectUnblocked -= OnObjectUnblocked;
		_timeTrigger.Pause();
		DisableComponent();
	}

	public void Save(IEntitySaver entitySaver)
	{
		entitySaver.GetComponent(HiveKey).Set(PollinationProgressKey, _timeTrigger.Progress);
	}

	public void Load(IEntityLoader entityLoader)
	{
		IObjectLoader component = entityLoader.GetComponent(HiveKey);
		_timeTrigger.FastForwardProgress(component.Get(PollinationProgressKey));
	}

	public IEnumerable<Vector3Int> GetBlocksInRange()
	{
		return _blockObjectRange.GetBlocksOnTerrainInRectangularRadius(_hiveSpec.PollinationRadius);
	}

	private void PollinateNearbyPollinatees()
	{
		UpdateNearbyPollinatees();
		int num = Mathf.Min(_hiveSpec.PlantsPerPollination, _nearbyPollinatees.Count);
		for (int i = 0; i < num; i++)
		{
			Pollinatee listElement = _randomNumberGenerator.GetListElement(_nearbyPollinatees);
			_nearbyPollinatees.Remove(listElement);
			listElement.Pollinate(_hiveSpec.GrowthTimeReduction);
		}
		_timeTrigger.Reset();
		_timeTrigger.Resume();
	}

	private void UpdateNearbyPollinatees()
	{
		_nearbyPollinatees.Clear();
		foreach (Vector3Int item in GetBlocksInRange())
		{
			Pollinatee bottomObjectComponentAt = _blockService.GetBottomObjectComponentAt<Pollinatee>(item);
			if ((bool)bottomObjectComponentAt && bottomObjectComponentAt.CanPollinate)
			{
				_nearbyPollinatees.Add(bottomObjectComponentAt);
			}
		}
	}

	private void OnObjectBlocked(object sender, EventArgs e)
	{
		_timeTrigger.Pause();
	}

	private void OnObjectUnblocked(object sender, EventArgs e)
	{
		_timeTrigger.Resume();
	}
}
