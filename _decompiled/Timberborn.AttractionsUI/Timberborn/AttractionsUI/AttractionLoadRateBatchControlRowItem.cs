using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.Attractions;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.TimeSystem;
using UnityEngine.UIElements;

namespace Timberborn.AttractionsUI;

internal class AttractionLoadRateBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem, IFinishableBatchControlRowItem
{
	private readonly IDayNightCycle _dayNightCycle;

	private readonly AttractionLoadRate _attractionLoadRate;

	private readonly ImmutableArray<VisualElement> _loadRateRoots;

	private ImmutableArray<VisualElement> _loadRates;

	private ImmutableArray<VisualElement> _hourMarkers;

	public VisualElement Root { get; }

	public AttractionLoadRateBatchControlRowItem(IDayNightCycle dayNightCycle, VisualElement root, AttractionLoadRate attractionLoadRate, IEnumerable<VisualElement> loadRateRoots)
	{
		_dayNightCycle = dayNightCycle;
		Root = root;
		_attractionLoadRate = attractionLoadRate;
		_loadRateRoots = loadRateRoots.ToImmutableArray();
	}

	public void Initialize()
	{
		_loadRates = _loadRateRoots.Select((VisualElement rate) => rate.Q<VisualElement>("Rate")).ToImmutableArray();
		_hourMarkers = _loadRateRoots.Select((VisualElement rate) => rate.Q<VisualElement>("CurrentHourMarker")).ToImmutableArray();
	}

	public void UpdateRowItem()
	{
		for (int i = 0; i < _loadRates.Length; i++)
		{
			VisualElement visualElement = _loadRates[i];
			VisualElement visualElement2 = _hourMarkers[i];
			float loadRate = _attractionLoadRate.GetLoadRate(i);
			bool visible = i == (int)_dayNightCycle.HoursPassedToday;
			visualElement.style.height = new StyleLength(Length.Percent(loadRate * 100f));
			visualElement2.ToggleDisplayStyle(visible);
		}
	}

	public void SetFinishedState(bool isFinished)
	{
		Root.ToggleDisplayStyle(isFinished);
	}
}
