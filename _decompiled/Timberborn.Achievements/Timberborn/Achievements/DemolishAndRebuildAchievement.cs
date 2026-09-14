using Timberborn.AchievementSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Coordinates;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using UnityEngine;

namespace Timberborn.Achievements;

internal class DemolishAndRebuildAchievement : Achievement
{
	private static readonly float TimeLimit = 60f;

	private readonly EventBus _eventBus;

	private string _lastDeletionId;

	private Placement _lastDeletionPlacement;

	private float _lastDeletionTime;

	public override string Id => "DEMOLISH_AND_REBUILD";

	protected DemolishAndRebuildAchievement(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	[OnEvent]
	public void OnBlockObjectSet(BlockObjectSetEvent blockObjectSetEvent)
	{
		BlockObject blockObject = blockObjectSetEvent.BlockObject;
		if (!blockObject.IsPreview && blockObject.HasComponent<Building>())
		{
			if (Time.unscaledTime - _lastDeletionTime < TimeLimit && IsSameBuilding(blockObject))
			{
				Unlock();
			}
			else
			{
				Reset();
			}
		}
	}

	[OnEvent]
	public void OnExitedFinishedStateEvent(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		BlockObject blockObject = exitedFinishedStateEvent.BlockObject;
		if (blockObject.HasComponent<Building>())
		{
			_lastDeletionPlacement = blockObject.Placement;
			_lastDeletionId = blockObject.GetComponent<TemplateSpec>().TemplateName;
			_lastDeletionTime = Time.unscaledTime;
		}
	}

	protected override void EnableInternal()
	{
		_eventBus.Register(this);
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}

	private bool IsSameBuilding(BlockObject blockObject)
	{
		if (!string.IsNullOrWhiteSpace(_lastDeletionId) && blockObject.GetComponent<TemplateSpec>().TemplateName == _lastDeletionId)
		{
			return blockObject.Placement == _lastDeletionPlacement;
		}
		return false;
	}

	private void Reset()
	{
		_lastDeletionId = null;
		_lastDeletionTime = 0f;
	}
}
