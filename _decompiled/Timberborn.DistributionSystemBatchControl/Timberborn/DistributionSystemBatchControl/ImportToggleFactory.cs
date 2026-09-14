using Timberborn.CoreUI;
using Timberborn.DistributionSystem;
using Timberborn.Localization;
using Timberborn.SliderToggleSystem;
using UnityEngine.UIElements;

namespace Timberborn.DistributionSystemBatchControl;

internal class ImportToggleFactory
{
	private static readonly string ImportDisabledIconClass = "import-icon--disabled";

	private static readonly string ImportAutoIconClass = "import-icon--auto";

	private static readonly string ImportForcedIconClass = "import-icon--forced";

	private static readonly string ImportDisabledBackgroundClass = "import-background--disabled";

	private static readonly string ImportAutoBackgroundClass = "import-background--auto";

	private static readonly string ImportForcedBackgroundClass = "import-background--forced";

	private static readonly string ImportDisabledLocKey = "Distribution.ImportDisabled";

	private static readonly string ImportDisabledDescriptionLocKey = "Distribution.ImportDisabled.Description";

	private static readonly string ImportAutoLocKey = "Distribution.ImportAuto";

	private static readonly string ImportAutoDescriptionLocKey = "Distribution.ImportAuto.Description";

	private static readonly string ImportForcedLocKey = "Distribution.ImportForced";

	private static readonly string ImportForcedDescriptionLocKey = "Distribution.ImportForced.Description";

	private static readonly string BalanceInfoLocKey = "Distribution.BalanceInfo";

	private readonly ILoc _loc;

	private readonly SliderToggleFactory _sliderToggleFactory;

	private readonly VisualElementLoader _visualElementLoader;

	public ImportToggleFactory(ILoc loc, SliderToggleFactory sliderToggleFactory, VisualElementLoader visualElementLoader)
	{
		_loc = loc;
		_sliderToggleFactory = sliderToggleFactory;
		_visualElementLoader = visualElementLoader;
	}

	public SliderToggle Create(VisualElement parent, GoodDistributionSetting setting)
	{
		SliderToggleItem sliderToggleItem = SliderToggleItem.Create(GetImportDisabledTooltip, ImportDisabledIconClass, ImportDisabledBackgroundClass, delegate
		{
			setting.SetImportOption(ImportOption.Disabled);
		}, () => setting.ImportOption == ImportOption.Disabled);
		SliderToggleItem sliderToggleItem2 = SliderToggleItem.Create(GetImportAutoTooltip, ImportAutoIconClass, ImportAutoBackgroundClass, delegate
		{
			setting.SetImportOption(ImportOption.Auto);
		}, () => setting.ImportOption == ImportOption.Auto);
		SliderToggleItem sliderToggleItem3 = SliderToggleItem.Create(GetImportForcedTooltip, ImportForcedIconClass, ImportForcedBackgroundClass, delegate
		{
			setting.SetImportOption(ImportOption.Forced);
		}, () => setting.ImportOption == ImportOption.Forced);
		return _sliderToggleFactory.Create(parent, sliderToggleItem, sliderToggleItem2, sliderToggleItem3);
	}

	private VisualElement GetImportDisabledTooltip()
	{
		return GetTooltip(ImportDisabledLocKey, ImportDisabledDescriptionLocKey, withBalanceInfo: false);
	}

	private VisualElement GetImportAutoTooltip()
	{
		return GetTooltip(ImportAutoLocKey, ImportAutoDescriptionLocKey, withBalanceInfo: true);
	}

	private VisualElement GetImportForcedTooltip()
	{
		return GetTooltip(ImportForcedLocKey, ImportForcedDescriptionLocKey, withBalanceInfo: true);
	}

	private VisualElement GetTooltip(string title, string description, bool withBalanceInfo)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/ImportToggleTooltip");
		visualElement.Q<Label>("Title").text = _loc.T(title);
		visualElement.Q<Label>("Description").text = (withBalanceInfo ? (_loc.T(description) + "\n" + _loc.T(BalanceInfoLocKey)) : _loc.T(description));
		return visualElement;
	}
}
