using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.GameDistrictsMigration;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class PopulationDistributorBatchControlRowItemFactory
{
	private static readonly string EmigrationDisabledLocKey = "Migration.AutomaticEmigrationDisabled";

	private static readonly string EmigrationEnabledLocKey = "Migration.AutomaticEmigrationEnabled";

	private static readonly string ImmigrationDisabledLocKey = "Migration.AutomaticImmigrationDisabled";

	private static readonly string ImmigrationEnabledLocKey = "Migration.AutomaticImmigrationEnabled";

	private static readonly string HighMinimumWarningLocKey = "Migration.HighMinimumWarning";

	private readonly AlternateClickableFactory _alternateClickableFactory;

	private readonly ILoc _loc;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	public PopulationDistributorBatchControlRowItemFactory(AlternateClickableFactory alternateClickableFactory, ILoc loc, ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader)
	{
		_alternateClickableFactory = alternateClickableFactory;
		_loc = loc;
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
	}

	public IBatchControlRowItem Create(PopulationDistributor populationDistributor)
	{
		string elementName = "Game/BatchControl/PopulationDistributorBatchControlRowItem";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		IntegerField integerField = visualElement.Q<IntegerField>("MinimumValue");
		var (decreaseMinimum, increaseMinimum) = InitializeMinimumControls(populationDistributor, integerField, visualElement);
		InitializeImmigrationToggle(populationDistributor, visualElement);
		InitializeEmigrationToggle(populationDistributor, visualElement);
		VisualElement visualElement2 = visualElement.Q<VisualElement>("NeedingIcon");
		_tooltipRegistrar.RegisterLocalizable(visualElement2, HighMinimumWarningLocKey);
		return new PopulationDistributorBatchControlRowItem(visualElement, integerField, decreaseMinimum, increaseMinimum, visualElement2, populationDistributor);
	}

	private (AlternateClickable, AlternateClickable) InitializeMinimumControls(PopulationDistributor populationDistributor, IntegerField minimumValue, VisualElement root)
	{
		TextFields.InitializeIntegerField(minimumValue, populationDistributor.Minimum, 0, int.MaxValue, populationDistributor.SetMinimumAndMigrate);
		return (_alternateClickableFactory.Create(root.Q<Button>("MinusButton"), delegate
		{
			ChangeMigrationMinimum(-1, populationDistributor);
		}, delegate
		{
			ChangeMigrationMinimum(-10, populationDistributor);
		}), _alternateClickableFactory.Create(root.Q<Button>("PlusButton"), delegate
		{
			ChangeMigrationMinimum(1, populationDistributor);
		}, delegate
		{
			ChangeMigrationMinimum(10, populationDistributor);
		}));
	}

	private void InitializeImmigrationToggle(PopulationDistributor populationDistributor, VisualElement root)
	{
		Toggle immigrationToggle = root.Q<Toggle>("ImmigrationToggle");
		immigrationToggle.SetValueWithoutNotify(populationDistributor.AllowImmigration);
		immigrationToggle.RegisterCallback<ChangeEvent<bool>>(delegate
		{
			populationDistributor.ToggleAllowImmigrationAndMigrate();
		});
		_tooltipRegistrar.Register(immigrationToggle, () => GetImmigrationToggleTooltip(immigrationToggle));
	}

	private void InitializeEmigrationToggle(PopulationDistributor populationDistributor, VisualElement root)
	{
		Toggle emigrationToggle = root.Q<Toggle>("EmigrationToggle");
		emigrationToggle.SetValueWithoutNotify(populationDistributor.AllowEmigration);
		emigrationToggle.RegisterCallback<ChangeEvent<bool>>(delegate
		{
			populationDistributor.ToggleAllowEmigrationAndMigrate();
		});
		_tooltipRegistrar.Register(emigrationToggle, () => GetEmigrationToggleTooltip(emigrationToggle));
	}

	private static void ChangeMigrationMinimum(int change, PopulationDistributor populationDistributor)
	{
		int minimumAndMigrate = populationDistributor.Minimum + change;
		populationDistributor.SetMinimumAndMigrate(minimumAndMigrate);
	}

	private string GetImmigrationToggleTooltip(Toggle toggle)
	{
		return _loc.T(toggle.value ? ImmigrationEnabledLocKey : ImmigrationDisabledLocKey);
	}

	private string GetEmigrationToggleTooltip(Toggle toggle)
	{
		return _loc.T(toggle.value ? EmigrationEnabledLocKey : EmigrationDisabledLocKey);
	}
}
