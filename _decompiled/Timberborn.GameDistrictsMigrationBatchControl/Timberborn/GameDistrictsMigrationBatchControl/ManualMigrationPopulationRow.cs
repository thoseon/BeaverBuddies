using System;
using Timberborn.CoreUI;
using Timberborn.GameDistricts;
using Timberborn.GameDistrictsMigration;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class ManualMigrationPopulationRow
{
	private readonly ManualMigrationBlocker _manualMigrationBlocker;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly Func<DistrictCenter, PopulationDistributor> _populationDistributorGetter;

	private Label _currentPopulationLabel;

	private Label _currentMinimumLabel;

	private PopulationDistributor _populationDistributor;

	private DistrictCenter _target;

	private Button _buttonOne;

	private Button _buttonTen;

	private Button _buttonAll;

	private readonly Func<bool> _visibilityGetter;

	public VisualElement Root { get; }

	public ManualMigrationPopulationRow(ManualMigrationBlocker manualMigrationBlocker, ITooltipRegistrar tooltipRegistrar, VisualElement root, Func<DistrictCenter, PopulationDistributor> populationDistributorGetter, Func<bool> visibilityGetter)
	{
		_manualMigrationBlocker = manualMigrationBlocker;
		_tooltipRegistrar = tooltipRegistrar;
		Root = root;
		_populationDistributorGetter = populationDistributorGetter;
		_visibilityGetter = visibilityGetter;
	}

	public void Initialize()
	{
		_currentPopulationLabel = Root.Q<Label>("CurrentPopulation");
		_currentMinimumLabel = Root.Q<Label>("CurrentMinimum");
		SetupButtons();
	}

	public void SetDistricts(DistrictCenter source, DistrictCenter target)
	{
		_populationDistributor = _populationDistributorGetter(source);
		_target = target;
		UpdateRow();
	}

	public void UpdateRow()
	{
		bool flag = _visibilityGetter();
		Root.ToggleDisplayStyle(flag);
		if (flag)
		{
			_currentPopulationLabel.text = _populationDistributor.Current.ToString();
			_currentMinimumLabel.text = _populationDistributor.Minimum.ToString();
			SetButtonsEnabledState();
		}
	}

	private void SetupButtons()
	{
		_buttonOne = Root.Q<Button>("ButtonOne");
		_buttonTen = Root.Q<Button>("ButtonTen");
		_buttonAll = Root.Q<Button>("ButtonAll");
		_buttonOne.RegisterCallback<ClickEvent>(delegate
		{
			MigratePopulation(1);
		});
		_buttonTen.RegisterCallback<ClickEvent>(delegate
		{
			MigratePopulation(10);
		});
		_buttonAll.RegisterCallback<ClickEvent>(delegate
		{
			MigratePopulation(_populationDistributor.Current);
		});
		_tooltipRegistrar.Register((VisualElement)_buttonOne, (Func<TooltipContent>)GetTooltip);
		_tooltipRegistrar.Register((VisualElement)_buttonTen, (Func<TooltipContent>)GetTooltip);
		_tooltipRegistrar.Register((VisualElement)_buttonAll, (Func<TooltipContent>)GetTooltip);
	}

	private TooltipContent GetTooltip()
	{
		if (_manualMigrationBlocker.IsEnabled)
		{
			return TooltipContent.CreateEmpty();
		}
		return TooltipContent.CreateInstant(_manualMigrationBlocker.TooltipText);
	}

	private void MigratePopulation(int amount)
	{
		amount = Math.Min(amount, _populationDistributor.Current);
		if (amount > 0)
		{
			_populationDistributor.MigrateToAndCheckAutomaticMigration(_target, amount);
		}
	}

	private void SetButtonsEnabledState()
	{
		_buttonOne.SetEnabled(_manualMigrationBlocker.IsEnabled);
		_buttonTen.SetEnabled(_manualMigrationBlocker.IsEnabled);
		_buttonAll.SetEnabled(_manualMigrationBlocker.IsEnabled);
	}
}
