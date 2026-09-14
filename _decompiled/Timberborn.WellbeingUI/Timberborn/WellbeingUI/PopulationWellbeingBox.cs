using System.Collections.Generic;
using Timberborn.CoreUI;
using Timberborn.FactionSystem;
using Timberborn.GameDistricts;
using Timberborn.Population;
using Timberborn.SingletonSystem;
using Timberborn.Wellbeing;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class PopulationWellbeingBox : IPanelController, ILoadableSingleton, IPanelBlocker
{
	private static readonly string WellbeingHighscoreClass = "wellbeing-highscore";

	private static readonly string NegativeWellbeingClass = "wellbeing--negative";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly WellbeingService _wellbeingService;

	private readonly PopulationService _populationService;

	private readonly DistrictContextService _districtContextService;

	private readonly PopulationWellbeingCounterGroupFactory _populationWellbeingCounterGroupFactory;

	private readonly PopulationWellbeingGoals _populationWellbeingGoals;

	private readonly List<PopulationWellbeingCounter> _counters = new List<PopulationWellbeingCounter>();

	private readonly Dictionary<string, int> _appliedCount = new Dictionary<string, int>();

	private VisualElement _root;

	private Label _averageWellbeing;

	private PopulationData ContextualPopulationData
	{
		get
		{
			if (!_districtContextService.SelectedDistrict)
			{
				return _populationService.GlobalPopulationData;
			}
			return _populationService.DistrictPopulationData;
		}
	}

	public PopulationWellbeingBox(VisualElementLoader visualElementLoader, PanelStack panelStack, WellbeingService wellbeingService, PopulationService populationService, DistrictContextService districtContextService, PopulationWellbeingCounterGroupFactory populationWellbeingCounterGroupFactory, PopulationWellbeingGoals populationWellbeingGoals)
	{
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_wellbeingService = wellbeingService;
		_populationService = populationService;
		_districtContextService = districtContextService;
		_populationWellbeingCounterGroupFactory = populationWellbeingCounterGroupFactory;
		_populationWellbeingGoals = populationWellbeingGoals;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/Population/PopulationWellbeingBox");
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		_averageWellbeing = _root.Q<Label>("AverageWellbeingScore");
		_populationWellbeingGoals.Initialize(_root);
		InitializeGroups(_root.Q<ScrollView>("Items"));
	}

	public VisualElement GetPanel()
	{
		UpdateAppliedNeedsCount();
		UpdateAverageWellbeing();
		UpdateCounters();
		_populationWellbeingGoals.AddGoals();
		return _root;
	}

	public void Show()
	{
		_panelStack.PushOverlay(this);
	}

	public void ShowWellbeingHighscore()
	{
		_root.AddToClassList(WellbeingHighscoreClass);
		_panelStack.PushOverlay(this);
	}

	public void ShowUnlockedFaction(FactionSpec factionSpec)
	{
		_panelStack.PushOverlay(this);
		_populationWellbeingGoals.StartBlinking(factionSpec);
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
		_root.RemoveFromClassList(WellbeingHighscoreClass);
		_appliedCount.Clear();
		_populationWellbeingGoals.Clear();
		_panelStack.Pop(this);
	}

	private void InitializeGroups(VisualElement root)
	{
		foreach (PopulationWellbeingCounterGroup item in _populationWellbeingCounterGroupFactory.Create())
		{
			root.Add(item.Root);
			_counters.AddRange(item.Counters);
		}
	}

	private void UpdateAppliedNeedsCount()
	{
		if ((bool)_districtContextService.SelectedDistrict)
		{
			_wellbeingService.DistrictAppliedNeeds(_appliedCount);
		}
		else
		{
			_wellbeingService.GlobalAppliedNeeds(_appliedCount);
		}
	}

	private void UpdateAverageWellbeing()
	{
		int num = (_districtContextService.SelectedDistrict ? _wellbeingService.AverageDistrictWellbeing : _wellbeingService.AverageGlobalWellbeing);
		_averageWellbeing.text = num.ToString();
		_averageWellbeing.EnableInClassList(NegativeWellbeingClass, num < 0);
	}

	private void UpdateCounters()
	{
		int numberOfBeavers = ContextualPopulationData.NumberOfBeavers;
		foreach (PopulationWellbeingCounter counter in _counters)
		{
			int valueOrDefault = _appliedCount.GetValueOrDefault(counter.NeedId, 0);
			counter.UpdateValues(valueOrDefault, numberOfBeavers);
		}
	}
}
