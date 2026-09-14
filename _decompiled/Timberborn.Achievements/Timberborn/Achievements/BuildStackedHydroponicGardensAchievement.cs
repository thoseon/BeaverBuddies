using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.AchievementSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.Achievements;

internal class BuildStackedHydroponicGardensAchievement : Achievement
{
	private static readonly int RequiredStackHeight = 8;

	private static readonly string TemplateName = "HydroponicGarden.IronTeeth";

	private readonly EventBus _eventBus;

	private readonly IBlockService _blockService;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly Queue<Vector3Int> _blockCache = new Queue<Vector3Int>();

	private readonly HashSet<Vector2Int> _firstCoordinates = new HashSet<Vector2Int>();

	private readonly HashSet<Vector2Int> _secondCoordinates = new HashSet<Vector2Int>();

	public override string Id => "BUILD_STACKED_HYDROPONIC_GARDENS";

	public BuildStackedHydroponicGardensAchievement(EventBus eventBus, IBlockService blockService, EntityComponentRegistry entityComponentRegistry)
	{
		_eventBus = eventBus;
		_blockService = blockService;
		_entityComponentRegistry = entityComponentRegistry;
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		if (IsValidStack(enteredFinishedStateEvent.BlockObject))
		{
			Unlock();
		}
	}

	protected override void EnableInternal()
	{
		if (HasValidStackOnStart())
		{
			Unlock();
		}
		else
		{
			_eventBus.Register(this);
		}
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}

	private bool HasValidStackOnStart()
	{
		foreach (Building item in from building in _entityComponentRegistry.GetEnabled<Building>()
			where building.GetComponent<BlockObject>().IsFinished && building.GetComponent<TemplateSpec>().TemplateName == TemplateName
			select building)
		{
			if (IsValidStack(item.GetComponent<BlockObject>()))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsValidBuilding(BlockObject blockObject)
	{
		if ((bool)blockObject && blockObject.IsFinished)
		{
			TemplateSpec component = blockObject.GetComponent<TemplateSpec>();
			if ((object)component != null)
			{
				return component.TemplateName == TemplateName;
			}
		}
		return false;
	}

	private bool IsValidStack(BlockObject blockObject)
	{
		if (IsValidBuilding(blockObject) && blockObject.Coordinates.z >= RequiredStackHeight - 1)
		{
			int num = 1;
			FillBlockCache(blockObject);
			do
			{
				BlockObject bottomObjectAt = _blockService.GetBottomObjectAt(_blockCache.Dequeue().Below());
				if (IsValidBuilding(bottomObjectAt) && HasSameFootprint(blockObject, bottomObjectAt))
				{
					num++;
					blockObject = bottomObjectAt;
					FillBlockCache(bottomObjectAt);
				}
			}
			while (_blockCache.Count > 0 && num < RequiredStackHeight);
			if (num >= RequiredStackHeight)
			{
				return true;
			}
		}
		return false;
	}

	private bool HasSameFootprint(BlockObject first, BlockObject second)
	{
		ImmutableArray<Block> allBlocks = first.PositionedBlocks.GetAllBlocks();
		ImmutableArray<Block> allBlocks2 = second.PositionedBlocks.GetAllBlocks();
		if (allBlocks.Length != allBlocks2.Length)
		{
			return false;
		}
		for (int i = 0; i < allBlocks.Length; i++)
		{
			_firstCoordinates.Add(allBlocks[i].Coordinates.XY());
			_secondCoordinates.Add(allBlocks2[i].Coordinates.XY());
		}
		bool result = _firstCoordinates.SetEquals(_secondCoordinates);
		_firstCoordinates.Clear();
		_secondCoordinates.Clear();
		return result;
	}

	private void FillBlockCache(BlockObject objectBelow)
	{
		_blockCache.Clear();
		foreach (Vector3Int foundationCoordinate in objectBelow.PositionedBlocks.GetFoundationCoordinates())
		{
			_blockCache.Enqueue(foundationCoordinate);
		}
	}
}
