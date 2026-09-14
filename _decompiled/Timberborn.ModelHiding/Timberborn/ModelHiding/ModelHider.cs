using System;
using System.Collections.Generic;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.ModelHiding;

internal class ModelHider : ILoadableSingleton, IModelAdder
{
	private readonly EventBus _eventBus;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly MapSize _mapSize;

	private readonly HidableModels _hidableModels;

	private readonly UndergroundModelHider _undergroundModelHider;

	private readonly UncoveredModelHider _uncoveredModelHider;

	private readonly FloorModelHider _floorModelHider;

	private readonly HashSet<BlockObjectModelController> _modelsToUnblock = new HashSet<BlockObjectModelController>();

	public ModelHider(EventBus eventBus, ILevelVisibilityService levelVisibilityService, MapSize mapSize, HidableModels hidableModels, UndergroundModelHider undergroundModelHider, UncoveredModelHider uncoveredModelHider, FloorModelHider floorModelHider)
	{
		_eventBus = eventBus;
		_levelVisibilityService = levelVisibilityService;
		_mapSize = mapSize;
		_hidableModels = hidableModels;
		_undergroundModelHider = undergroundModelHider;
		_uncoveredModelHider = uncoveredModelHider;
		_floorModelHider = floorModelHider;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnEntityInitialized(EntityInitializedEvent entityInitializedEvent)
	{
		BlockObjectModelController component = entityInitializedEvent.Entity.GetComponent<BlockObjectModelController>();
		if (component != null)
		{
			AddModel(component);
		}
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		BlockObjectModelController component = entityDeletedEvent.Entity.GetComponent<BlockObjectModelController>();
		if (component != null)
		{
			RemoveModel(component);
		}
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		BlockObject blockObject = enteredFinishedStateEvent.BlockObject;
		if (!blockObject.GetComponent<EntityComponent>().Deleted)
		{
			BlockObjectModelController component = blockObject.GetComponent<BlockObjectModelController>();
			if (component != null)
			{
				AddModel(component);
				UpdateLevelsWithAnythingHidable();
			}
		}
	}

	[OnEvent]
	public void OnMaxVisibleLevelChanged(MaxVisibleLevelChangedEvent maxVisibleLevelChangedEvent)
	{
		int oldMaxVisibleLevel = maxVisibleLevelChangedEvent.OldMaxVisibleLevel;
		int maxVisibleLevel = _levelVisibilityService.MaxVisibleLevel;
		UpdateVisibility(oldMaxVisibleLevel, maxVisibleLevel);
	}

	public void AddModel(BlockObjectModelController model)
	{
		if (FitsInMap(model))
		{
			ResetModel(model);
			if (!_levelVisibilityService.BlockIsVisible(model.BlockObject.CoordinatesAtBaseZ))
			{
				model.BlockModel();
			}
			_hidableModels.Add(model);
			_uncoveredModelHider.ShowModelIfPossible(model);
			_undergroundModelHider.ShowModelIfPossible(model);
			_floorModelHider.ShowModelIfPossible(model);
			UpdateLevelsWithAnythingHidable();
		}
	}

	public void RemoveModel(BlockObjectModelController model)
	{
		if (FitsInMap(model))
		{
			ResetModel(model);
			_hidableModels.Remove(model);
			UpdateLevelsWithAnythingHidable();
		}
	}

	private bool FitsInMap(BlockObjectModelController model)
	{
		return model.BlockObject.GetTopLevel() <= _mapSize.TotalSize.z;
	}

	private static void ResetModel(BlockObjectModelController model)
	{
		model.UnblockModel();
		model.HideUncoveredModel();
		model.HideUndergroundModel();
	}

	private void UpdateLevelsWithAnythingHidable()
	{
		if (_hidableModels.TryGetHidableRange(out var maxLevel))
		{
			_levelVisibilityService.SetLevelsWithAnythingHidable(maxLevel);
		}
		else
		{
			_levelVisibilityService.SetLevelsWithAnythingHidable(0);
		}
	}

	private void UpdateVisibility(int oldLevel, int newLevel)
	{
		int minLevel = Math.Max(0, Math.Min(oldLevel, newLevel) - 1);
		int maxLevel = Math.Min(_mapSize.TotalSize.z, Math.Max(oldLevel, newLevel) + 1);
		UpdateBaseVisibility(minLevel, maxLevel, _modelsToUnblock);
		_undergroundModelHider.UpdateVisibility(minLevel, maxLevel, _modelsToUnblock);
		_uncoveredModelHider.UpdateVisibility(minLevel, maxLevel);
		_floorModelHider.UpdateVisibility(minLevel, maxLevel, _modelsToUnblock);
		UpdateModelBlockage(minLevel, maxLevel);
		_modelsToUnblock.Clear();
	}

	private void UpdateBaseVisibility(int minLevel, int maxLevel, ICollection<BlockObjectModelController> modelsToUnblock)
	{
		for (int i = minLevel; i <= maxLevel; i++)
		{
			foreach (BlockObjectModelController item in _hidableModels.ModelsAt(i))
			{
				BlockObject blockObject = item.BlockObject;
				if (_levelVisibilityService.BlockIsVisible(blockObject.CoordinatesAtBaseZ))
				{
					modelsToUnblock.Add(item);
				}
			}
		}
	}

	private void UpdateModelBlockage(int minLevel, int maxLevel)
	{
		for (int i = minLevel; i <= maxLevel; i++)
		{
			foreach (BlockObjectModelController item in _hidableModels.ModelsAt(i))
			{
				if (_modelsToUnblock.Contains(item))
				{
					item.UnblockModel();
				}
				else
				{
					item.BlockModel();
				}
			}
		}
	}
}
