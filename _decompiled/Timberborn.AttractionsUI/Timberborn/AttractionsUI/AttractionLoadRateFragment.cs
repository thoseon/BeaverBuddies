using System.Collections.Generic;
using Timberborn.Attractions;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.TimeSystem;
using UnityEngine.UIElements;

namespace Timberborn.AttractionsUI;

public class AttractionLoadRateFragment : IEntityPanelFragment
{
	private readonly struct LoadRate
	{
		public VisualElement Rate { get; }

		public VisualElement CurrentHourMarker { get; }

		public LoadRate(VisualElement rate, VisualElement currentHourMarker)
		{
			Rate = rate;
			CurrentHourMarker = currentHourMarker;
		}
	}

	private readonly VisualElementLoader _visualElementLoader;

	private readonly IDayNightCycle _dayNightCycle;

	private VisualElement _root;

	private readonly List<LoadRate> _loadRates = new List<LoadRate>();

	private AttractionLoadRate _attractionLoadRate;

	public AttractionLoadRateFragment(VisualElementLoader visualElementLoader, IDayNightCycle dayNightCycle)
	{
		_visualElementLoader = visualElementLoader;
		_dayNightCycle = dayNightCycle;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/AttractionLoadRateFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		VisualElement visualElement = _root.Q<VisualElement>("LoadRates");
		for (int i = 0; i < 24; i++)
		{
			VisualElement visualElement2 = _visualElementLoader.LoadVisualElement("Game/AttractionLoadRate");
			visualElement.Add(visualElement2);
			LoadRate item = new LoadRate(visualElement2.Q<VisualElement>("Rate"), visualElement2.Q<VisualElement>("CurrentHourMarker"));
			_loadRates.Add(item);
		}
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_attractionLoadRate = entity.GetComponent<AttractionLoadRate>();
	}

	public void ClearFragment()
	{
		_attractionLoadRate = null;
	}

	public void UpdateFragment()
	{
		if ((bool)(BaseComponent)(object)_attractionLoadRate && ((BaseComponent)(object)_attractionLoadRate).Enabled)
		{
			for (int i = 0; i < _loadRates.Count; i++)
			{
				UpdateLoadRate(i);
			}
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void UpdateLoadRate(int hour)
	{
		LoadRate loadRate = _loadRates[hour];
		float loadRate2 = _attractionLoadRate.GetLoadRate(hour);
		loadRate.Rate.style.height = new StyleLength(Length.Percent(loadRate2 * 100f));
		bool visible = hour == (int)_dayNightCycle.HoursPassedToday;
		loadRate.CurrentHourMarker.ToggleDisplayStyle(visible);
	}
}
