using System;
using System.Collections.Generic;
using Timberborn.AchievementSystem;
using Timberborn.BlockSystem;
using Timberborn.SingletonSystem;
using Timberborn.WaterBuildings;
using Timberborn.WaterObjects;

namespace Timberborn.Achievements;

internal class FloodBuildingAchievement : Achievement
{
	private readonly EventBus _eventBus;

	private readonly HashSet<FloodableObject> _floodableBuildings = new HashSet<FloodableObject>();

	public override string Id => "FLOOD_BUILDING";

	public FloodBuildingAchievement(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		BlockObject blockObject = enteredFinishedStateEvent.BlockObject;
		if (!blockObject.HasComponent<FloodableBuilding>())
		{
			return;
		}
		FloodableObject component = blockObject.GetComponent<FloodableObject>();
		if (component != null)
		{
			if (component.IsFlooded)
			{
				Unlock();
				return;
			}
			_floodableBuildings.Add(component);
			component.Flooded += OnFlooded;
		}
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		BlockObject blockObject = exitedFinishedStateEvent.BlockObject;
		if (blockObject.HasComponent<FloodableBuilding>())
		{
			FloodableObject component = blockObject.GetComponent<FloodableObject>();
			if (component != null)
			{
				_floodableBuildings.Remove(component);
				component.Flooded -= OnFlooded;
			}
		}
	}

	protected override void EnableInternal()
	{
		_eventBus.Register(this);
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
		foreach (FloodableObject floodableBuilding in _floodableBuildings)
		{
			floodableBuilding.Flooded -= OnFlooded;
		}
	}

	private void OnFlooded(object sender, EventArgs e)
	{
		Unlock();
	}
}
