using System.Collections.Generic;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.GameFactionSystem;
using Timberborn.NeedSpecs;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class PopulationWellbeingCounterGroupFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly NeedGroupSpecService _needGroupSpecService;

	private readonly FactionNeedService _factionNeedService;

	public PopulationWellbeingCounterGroupFactory(VisualElementLoader visualElementLoader, NeedGroupSpecService needGroupSpecService, FactionNeedService factionNeedService)
	{
		_visualElementLoader = visualElementLoader;
		_needGroupSpecService = needGroupSpecService;
		_factionNeedService = factionNeedService;
	}

	public IEnumerable<PopulationWellbeingCounterGroup> Create()
	{
		foreach (NeedGroupSpec needGroup in _needGroupSpecService.NeedGroups)
		{
			string elementName = "Game/Population/PopulationWellbeingCounterGroup";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			visualElement.Q<Label>("Header").text = needGroup.DisplayName.Value;
			IEnumerable<PopulationWellbeingCounter> counters = Create(visualElement.Q<VisualElement>("Items"), needGroup.Id);
			PopulationWellbeingCounterGroup populationWellbeingCounterGroup = new PopulationWellbeingCounterGroup(visualElement, counters);
			if (populationWellbeingCounterGroup.HasCounters)
			{
				yield return populationWellbeingCounterGroup;
			}
		}
	}

	private IEnumerable<PopulationWellbeingCounter> Create(VisualElement root, string needGroupId)
	{
		IEnumerable<IGrouping<string, NeedSpec>> enumerable = from need in _factionNeedService.GetBeaverNeeds()
			where need.NeedGroupId == needGroupId && need.AffectsWellbeing
			group need by need.Id;
		foreach (IGrouping<string, NeedSpec> item in enumerable)
		{
			string elementName = "Game/Population/PopulationWellbeingCounter";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			root.Add(visualElement);
			NeedSpec needSpec = item.First();
			VisualElement bar = visualElement.Q<VisualElement>("Progress");
			VisualElement barWrapper = visualElement.Q<VisualElement>("ProgressWrapper");
			Label appliedCount = visualElement.Q<Label>("Count");
			Label averageWellbeingShare = visualElement.Q<Label>("AverageWellbeingShare");
			PopulationWellbeingCounter populationWellbeingCounter = new PopulationWellbeingCounter(needSpec, visualElement, bar, barWrapper, appliedCount, averageWellbeingShare);
			visualElement.Q<Label>("Text").text = needSpec.DisplayName.Value;
			yield return populationWellbeingCounter;
		}
	}
}
