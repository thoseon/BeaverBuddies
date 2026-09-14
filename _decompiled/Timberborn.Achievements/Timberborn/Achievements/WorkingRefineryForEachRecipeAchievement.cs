using System.Collections.Generic;
using System.Linq;
using Timberborn.AchievementSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.GameFactionSystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.TickSystem;
using Timberborn.Workshops;

namespace Timberborn.Achievements;

internal class WorkingRefineryForEachRecipeAchievement : Achievement, ITickableSingleton
{
	private static readonly string TemplateId = "Refinery.Folktails";

	private readonly EventBus _eventBus;

	private readonly EntityComponentRegistry _entityComponentRegistry;

	private readonly FactionService _factionService;

	private readonly TemplateService _templateService;

	private readonly HashSet<Manufactory> _refineries = new HashSet<Manufactory>();

	private readonly HashSet<string> _uniqueRecipes = new HashSet<string>();

	private int _requiredRecipesCount;

	public override string Id => "WORKING_REFINERY_FOR_EACH_RECIPE";

	public WorkingRefineryForEachRecipeAchievement(EventBus eventBus, EntityComponentRegistry entityComponentRegistry, FactionService factionService, TemplateService templateService)
	{
		_eventBus = eventBus;
		_entityComponentRegistry = entityComponentRegistry;
		_factionService = factionService;
		_templateService = templateService;
	}

	public void Tick()
	{
		if (base.IsEnabled)
		{
			CheckUnlockCondition();
		}
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		BlockObject blockObject = enteredFinishedStateEvent.BlockObject;
		TemplateSpec component = blockObject.GetComponent<TemplateSpec>();
		if ((object)component != null && component.TemplateName == TemplateId)
		{
			_refineries.Add(blockObject.GetComponent<Manufactory>());
		}
	}

	[OnEvent]
	public void OnExitedFinishedState(ExitedFinishedStateEvent exitedFinishedStateEvent)
	{
		BlockObject blockObject = exitedFinishedStateEvent.BlockObject;
		TemplateSpec component = blockObject.GetComponent<TemplateSpec>();
		if ((object)component != null && component.TemplateName == TemplateId)
		{
			_refineries.Remove(blockObject.GetComponent<Manufactory>());
		}
	}

	protected override void EnableInternal()
	{
		if (_factionService.Current.Id == AchievementHelper.Folktails)
		{
			_eventBus.Register(this);
			_requiredRecipesCount = _templateService.GetAll<ManufactorySpec>().Single((ManufactorySpec m) => m.GetSpec<TemplateSpec>().TemplateName == TemplateId).ProductionRecipeIds.Length;
			ValidateInitialCount();
		}
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}

	private void CheckUnlockCondition()
	{
		foreach (Manufactory refinery in _refineries)
		{
			if (refinery.IsReadyToProduce && refinery.ProductionProgress > 0f && _uniqueRecipes.Add(refinery.CurrentRecipe.Id) && _uniqueRecipes.Count >= _requiredRecipesCount)
			{
				Unlock();
				return;
			}
		}
		_uniqueRecipes.Clear();
	}

	private void ValidateInitialCount()
	{
		foreach (Manufactory item in from manufactory in _entityComponentRegistry.GetEnabled<Manufactory>()
			where manufactory.GetComponent<BlockObject>().IsFinished
			where manufactory.GetComponent<TemplateSpec>().TemplateName == TemplateId
			select manufactory)
		{
			_refineries.Add(item);
		}
	}
}
