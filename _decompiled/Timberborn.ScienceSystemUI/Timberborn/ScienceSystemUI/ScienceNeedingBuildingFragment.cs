using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.ScienceSystem;
using Timberborn.TooltipSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.ScienceSystemUI;

public class ScienceNeedingBuildingFragment : IEntityPanelFragment
{
	private readonly ScienceCostPerHourFactory _scienceCostPerHourFactory;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	private ScienceNeedingBuilding _scienceNeedingBuilding;

	private ScienceNeedingBuildingDescriber _scienceNeedingBuildingDescriber;

	private VisualElement _root;

	private Timberborn.CoreUI.ProgressBar _progressBar;

	private ScienceCostPerHour _scienceCostPerHour;

	public ScienceNeedingBuildingFragment(ScienceCostPerHourFactory scienceCostPerHourFactory, ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader)
	{
		_scienceCostPerHourFactory = scienceCostPerHourFactory;
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/ScienceNeedingBuildingFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_progressBar = _root.Q<Timberborn.CoreUI.ProgressBar>("ProgressBar");
		_scienceCostPerHour = _scienceCostPerHourFactory.Create();
		_progressBar.Add(_scienceCostPerHour.Root);
		_tooltipRegistrar.Register(_root, (Func<string>)GetTooltipText);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_scienceNeedingBuilding = entity.GetComponent<ScienceNeedingBuilding>();
		if ((bool)(BaseComponent)(object)_scienceNeedingBuilding)
		{
			_scienceNeedingBuildingDescriber = entity.GetComponent<ScienceNeedingBuildingDescriber>();
		}
	}

	public void UpdateFragment()
	{
		if ((bool)(BaseComponent)(object)_scienceNeedingBuilding && ((BaseComponent)(object)_scienceNeedingBuilding).Enabled)
		{
			float progress = Mathf.Clamp01(_scienceNeedingBuilding.ScienceStoredPercentage);
			_scienceCostPerHour.UpdateCost(_scienceNeedingBuilding.ScienceUsedPerHour);
			_progressBar.SetProgress(progress);
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	public void ClearFragment()
	{
		_root.ToggleDisplayStyle(visible: false);
		_scienceNeedingBuilding = null;
	}

	private string GetTooltipText()
	{
		return _scienceNeedingBuildingDescriber.DescribeScienceUsage();
	}
}
