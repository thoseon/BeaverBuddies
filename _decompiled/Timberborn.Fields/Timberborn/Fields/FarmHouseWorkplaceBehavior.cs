using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Emptying;
using Timberborn.InventorySystem;
using Timberborn.Planting;
using Timberborn.SimpleOutputBuildings;
using Timberborn.WorkSystem;
using Timberborn.Yielding;

namespace Timberborn.Fields;

internal class FarmHouseWorkplaceBehavior : WorkplaceBehavior, IAwakableComponent
{
	private FarmHouse _farmHouse;

	private PlantablePrioritizer _plantablePrioritizer;

	private PlanterBuildingStatusUpdater _planterBuildingStatusUpdater;

	private Inventory _inventory;

	private EmptyOutputWorkplaceBehavior _emptyOutputWorkplaceBehavior;

	private InRangeYielders _inRangeYielders;

	public void Awake()
	{
		_farmHouse = GetComponent<FarmHouse>();
		_plantablePrioritizer = GetComponent<PlantablePrioritizer>();
		_planterBuildingStatusUpdater = GetComponent<PlanterBuildingStatusUpdater>();
		_inventory = GetComponent<SimpleOutputInventory>().Inventory;
		_emptyOutputWorkplaceBehavior = GetComponent<EmptyOutputWorkplaceBehavior>();
		_inRangeYielders = GetComponent<InRangeYielders>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (_plantablePrioritizer.PrioritizedPlantableSpec != null)
		{
			string templateName = _plantablePrioritizer.PrioritizedPlantableSpec.TemplateName;
			Decision result = Decide(agent, templateName);
			if (!result.ShouldReleaseNow)
			{
				return result;
			}
		}
		return Decide(agent, null);
	}

	private Decision Decide(BehaviorAgent agent, string prioritizedName)
	{
		PlantBehavior plantBehavior = agent.GetComponent<PlantBehavior>();
		HarvestStarter harvestStarter = agent.GetComponent<HarvestStarter>();
		return Decide(plantBehavior, () => plantBehavior.StartPlanting(agent), () => harvestStarter.StartHarvesting(_inventory, _inRangeYielders, prioritizedName));
	}

	private Decision Decide(Behavior plantBehavior, Func<Decision> plantDecisionGetter, Func<Decision> harvestDecisionGetter)
	{
		if (_farmHouse.PlantingPrioritized)
		{
			if (Decide(plantBehavior, plantDecisionGetter, out var decision))
			{
				return decision;
			}
			Decision result = harvestDecisionGetter();
			if (!result.ShouldReleaseNow)
			{
				_planterBuildingStatusUpdater.DeactivateStatus();
				return result;
			}
		}
		else
		{
			Decision result2 = harvestDecisionGetter();
			if (!result2.ShouldReleaseNow)
			{
				_planterBuildingStatusUpdater.DeactivateStatus();
				return result2;
			}
			BehaviorAgent component = plantBehavior.GetComponent<BehaviorAgent>();
			Decision decision2 = _emptyOutputWorkplaceBehavior.Decide(component);
			if (!decision2.ShouldReleaseNow)
			{
				return Decision.TransferNow(_emptyOutputWorkplaceBehavior, in decision2);
			}
			if (Decide(plantBehavior, plantDecisionGetter, out var decision3))
			{
				return decision3;
			}
		}
		_planterBuildingStatusUpdater.UpdateStatus();
		return Decision.ReleaseNow();
	}

	private bool Decide(Behavior behavior, Func<Decision> getDecision, out Decision decision)
	{
		Decision decision2 = getDecision();
		if (!decision2.ShouldReleaseNow)
		{
			_planterBuildingStatusUpdater.DeactivateStatus();
			decision = Decision.TransferNow(behavior, in decision2);
			return true;
		}
		decision = default(Decision);
		return false;
	}
}
