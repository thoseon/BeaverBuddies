using Timberborn.BaseComponentSystem;
using Timberborn.Buildings;
using Timberborn.BuildingsNavigation;
using Timberborn.GameDistricts;
using Timberborn.Navigation;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.DistanceHeatmap;

internal class DistanceHeatmapShower : BaseComponent, IAwakableComponent
{
	private static readonly float DarkeningFactor = 0.5f;

	private readonly Highlighter _highlighter;

	private readonly DistanceToColorConverter _distanceToColorConverter;

	private DistrictBuildingRegistry _districtBuildingRegistry;

	private Accessible _accessible;

	public DistanceHeatmapShower(Highlighter highlighter, DistanceToColorConverter distanceToColorConverter)
	{
		_highlighter = highlighter;
		_distanceToColorConverter = distanceToColorConverter;
	}

	public void Awake()
	{
		_districtBuildingRegistry = GetComponent<DistrictBuildingRegistry>();
		_districtBuildingRegistry.FinishedBuildingInstantRegistered += OnFinishedBuildingInstantRegistered;
		_districtBuildingRegistry.FinishedBuildingInstantUnregistered += OnFinishedBuildingInstantUnregistered;
		_accessible = GetComponent<BuildingAccessible>().Accessible;
		DisableComponent();
	}

	public void ShowHeatmap()
	{
		foreach (BuildingAccessible item in _districtBuildingRegistry.GetEnabledBuildingsInstant<BuildingAccessible>())
		{
			Highlight(item);
		}
		EnableComponent();
	}

	public void HideHeatmap()
	{
		_highlighter.UnhighlightAllSecondary();
		DisableComponent();
	}

	private void Highlight(BuildingAccessible buildingAccessible)
	{
		if (!_accessible.FindRoadPath(buildingAccessible.Accessible, out var distance))
		{
			_accessible.FindInstantRoadPath(buildingAccessible.Accessible, out distance);
		}
		if (distance > 0f)
		{
			Color color = _distanceToColorConverter.DistanceToColor((int)distance);
			_highlighter.HighlightSecondary(buildingAccessible, color * DarkeningFactor);
		}
	}

	private void OnFinishedBuildingInstantRegistered(object sender, FinishedBuildingInstantRegisteredEventArgs e)
	{
		if (base.Enabled)
		{
			BuildingAccessible component = e.Building.GetComponent<BuildingAccessible>();
			if (component != null)
			{
				Highlight(component);
			}
		}
	}

	private void OnFinishedBuildingInstantUnregistered(object sender, FinishedBuildingInstantUnregisteredEventArgs e)
	{
		if (base.Enabled)
		{
			_highlighter.UnhighlightSecondary(e.Building);
		}
	}
}
