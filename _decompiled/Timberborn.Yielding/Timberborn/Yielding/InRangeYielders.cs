using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuildingsNavigation;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.Yielding;

public class InRangeYielders : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly IBlockService _blockService;

	private readonly EventBus _eventBus;

	private YieldRemovingBuilding _yieldRemovingBuilding;

	private IYielderRetriever _yielderRetriever;

	private BuildingTerrainRange _buildingTerrainRange;

	private readonly List<Yielder> _yieldersInRange = new List<Yielder>();

	private bool _dirty;

	public event EventHandler YieldersChanged;

	public event EventHandler<Yielder> YielderAdded;

	public InRangeYielders(IBlockService blockService, EventBus eventBus)
	{
		_blockService = blockService;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_yieldRemovingBuilding = GetComponent<YieldRemovingBuilding>();
		_yielderRetriever = GetComponent<IYielderRetriever>();
		_buildingTerrainRange = GetComponent<BuildingTerrainRange>();
	}

	public void OnEnterFinishedState()
	{
		_buildingTerrainRange.RangeChanged += OnRangeChanged;
		_eventBus.Register(this);
	}

	public void OnExitFinishedState()
	{
		_buildingTerrainRange.RangeChanged -= OnRangeChanged;
		_eventBus.Unregister(this);
	}

	[OnEvent]
	public void OnEntityInitialized(EntityInitializedEvent entityInitializedEvent)
	{
		if (_yielderRetriever.TryGetYielder(entityInitializedEvent.Entity, out var yielder) && _buildingTerrainRange.GetRange().Contains(yielder.Coordinates) && IsAllowed(yielder))
		{
			_yieldersInRange.Add(yielder);
			YielderAdded?.Invoke(this, yielder);
		}
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		if (_yielderRetriever.TryGetYielder(entityDeletedEvent.Entity, out var yielder) && _yieldersInRange.Remove(yielder))
		{
			InvokeYieldersChanged();
		}
	}

	public void GetYields(HashSet<string> yields)
	{
		UpdateYielders(postEvent: false);
		for (int i = 0; i < _yieldersInRange.Count; i++)
		{
			Yielder yielder = _yieldersInRange[i];
			if ((bool)yielder)
			{
				yields.Add(yielder.YielderSpec.Yield.Id);
			}
		}
	}

	public bool GetYielders(IList<Yielder> yielders, string templateName = null)
	{
		bool result = false;
		UpdateYielders();
		for (int i = 0; i < _yieldersInRange.Count; i++)
		{
			Yielder yielder = _yieldersInRange[i];
			if (IsValidYielder(templateName, yielder))
			{
				result = true;
				if (!yielder.Reservable.Reserved)
				{
					yielders.Add(yielder);
				}
			}
		}
		return result;
	}

	private void OnRangeChanged(object sender, RangeChangedEventArgs rangeChangedEventArgs)
	{
		_dirty = true;
		if (rangeChangedEventArgs.IsInitialChange)
		{
			UpdateYielders();
		}
	}

	private void UpdateYielders(bool postEvent = true)
	{
		if (!_dirty)
		{
			return;
		}
		_yieldersInRange.Clear();
		foreach (Vector3Int item in _buildingTerrainRange.GetRange())
		{
			BlockObject bottomObjectAt = _blockService.GetBottomObjectAt(item);
			if ((bool)bottomObjectAt)
			{
				EntityComponent component = bottomObjectAt.GetComponent<EntityComponent>();
				if (component != null && component.Initialized && _yielderRetriever.TryGetYielder(component, out var yielder) && IsAllowed(yielder))
				{
					_yieldersInRange.Add(yielder);
				}
			}
		}
		_dirty = false;
		if (postEvent)
		{
			InvokeYieldersChanged();
		}
	}

	private static bool IsValidYielder(string templateName, Yielder yielder)
	{
		if ((bool)yielder)
		{
			if (templateName != null)
			{
				return yielder.GetComponent<TemplateSpec>().IsNamed(templateName);
			}
			return true;
		}
		return false;
	}

	private bool IsAllowed(Yielder yielder)
	{
		return _yieldRemovingBuilding.IsAllowed(yielder.YielderSpec);
	}

	private void InvokeYieldersChanged()
	{
		YieldersChanged?.Invoke(this, EventArgs.Empty);
	}
}
