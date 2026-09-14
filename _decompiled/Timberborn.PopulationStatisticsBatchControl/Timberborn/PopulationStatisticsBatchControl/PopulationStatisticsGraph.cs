using System.Collections.Generic;
using System.Linq;
using Timberborn.BatchControl;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.PopulationStatisticsSampling;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.PopulationStatisticsBatchControl;

internal class PopulationStatisticsGraph : IBatchControlRowItem, IInitializableBatchControlItem, IUpdatableBatchControlRowItem, IClearableBatchControlRowItem
{
	private readonly EventBus _eventBus;

	private readonly BatchControlBoxTimeRangeController _batchControlBoxTimeRangeController;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PopulationGraphState _populationGraphState;

	private readonly PopulationStatisticsTooltipRegistrar _populationStatisticsTooltipRegistrar;

	private Graph _graph;

	private readonly PopulationSampleHistory _populationSampleHistory;

	private readonly List<PopulationGraphDefinition> _graphDefinitions;

	private bool _isDirty;

	public VisualElement Root { get; }

	public PopulationStatisticsGraph(EventBus eventBus, BatchControlBoxTimeRangeController batchControlBoxTimeRangeController, VisualElementLoader visualElementLoader, PopulationGraphState populationGraphState, PopulationStatisticsTooltipRegistrar populationStatisticsTooltipRegistrar, VisualElement root, PopulationSampleHistory populationSampleHistory, List<PopulationGraphDefinition> graphDefinitions)
	{
		_eventBus = eventBus;
		_batchControlBoxTimeRangeController = batchControlBoxTimeRangeController;
		_visualElementLoader = visualElementLoader;
		_populationGraphState = populationGraphState;
		_populationStatisticsTooltipRegistrar = populationStatisticsTooltipRegistrar;
		Root = root;
		_populationSampleHistory = populationSampleHistory;
		_graphDefinitions = graphDefinitions;
	}

	public void InitializeRowItem()
	{
		_graph = Root.Q<Graph>("Graph");
		VisualElement visualElement = Root.Q<VisualElement>("Toggles");
		foreach (PopulationGraphDefinition graphDefinition in _graphDefinitions)
		{
			_graph.AddGraphDefinition(graphDefinition.GraphDefinition, RegisterTooltip);
			Toggle toggle = CreateToggle(graphDefinition);
			visualElement.Add(toggle);
			graphDefinition.GraphDefinition.SetVisibility(toggle.value);
		}
		UpdateItem();
		_eventBus.Register(this);
	}

	public void UpdateRowItem()
	{
		if (_isDirty)
		{
			UpdateItem();
			_isDirty = false;
		}
	}

	public void ClearRowItem()
	{
		_eventBus.Unregister(this);
		_graph.ClearGraphDefinitions();
		ClearDefinitions();
	}

	[OnEvent]
	public void OnPopulationSampled(PopulationSampledEvent populationSampledEvent)
	{
		_isDirty = true;
	}

	[OnEvent]
	public void OnBatchControlBoxTimeRangeChanged(BatchControlBoxTimeRangeChangedEvent batchControlBoxTimeRangeChangedEvent)
	{
		_isDirty = true;
	}

	private Toggle CreateToggle(PopulationGraphDefinition populationGraphDefinition)
	{
		string elementName = "Game/BatchControl/PopulationStatisticsToggle";
		Toggle toggle = _visualElementLoader.LoadVisualElement(elementName).Q<Toggle>("Toggle");
		string id = populationGraphDefinition.Id;
		toggle.text = populationGraphDefinition.GetDisplayName();
		toggle.value = (_populationGraphState.HasState(id) ? _populationGraphState.GetState(id) : populationGraphDefinition.GraphDefinition.IsVisible);
		toggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> evt)
		{
			OnCheckboxValueChanged(populationGraphDefinition, evt.newValue);
		});
		return toggle;
	}

	private void UpdateItem()
	{
		int count = _populationSampleHistory.PopulationSamples.Count;
		int num = (_batchControlBoxTimeRangeController.AllTimeSelected ? count : _batchControlBoxTimeRangeController.GetDaysTimeRange());
		ClearDefinitions();
		if (count < num)
		{
			CreateEmptyPoints(num - count);
		}
		if (count == num && count == 1)
		{
			CreateEmptyPoints(1);
		}
		foreach (PopulationSample item in _populationSampleHistory.PopulationSamples.TakeLast(num))
		{
			foreach (PopulationGraphDefinition graphDefinition in _graphDefinitions)
			{
				int valueById = item.GetValueById(graphDefinition.Id);
				graphDefinition.GraphDefinition.AddDataPoint(valueById);
			}
		}
		_graph.MarkDirtyRepaint();
	}

	private void ClearDefinitions()
	{
		foreach (PopulationGraphDefinition graphDefinition in _graphDefinitions)
		{
			graphDefinition.GraphDefinition.Clear();
		}
	}

	private void CreateEmptyPoints(int points)
	{
		for (int i = 0; i < points; i++)
		{
			foreach (PopulationGraphDefinition graphDefinition in _graphDefinitions)
			{
				graphDefinition.GraphDefinition.AddDataPoint(0f);
			}
		}
	}

	private void OnCheckboxValueChanged(PopulationGraphDefinition populationGraphDefinition, bool newValue)
	{
		populationGraphDefinition.GraphDefinition.SetVisibility(newValue);
		_populationGraphState.SetState(populationGraphDefinition.Id, newValue);
		_graph.MarkDirtyRepaint();
	}

	private void RegisterTooltip(VisualElement ve, int i)
	{
		_populationStatisticsTooltipRegistrar.Register(_populationSampleHistory, _graphDefinitions.AsReadOnlyList(), ve, i);
	}
}
