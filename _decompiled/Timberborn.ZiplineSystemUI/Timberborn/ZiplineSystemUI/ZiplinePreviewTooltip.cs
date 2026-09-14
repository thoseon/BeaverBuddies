using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using Timberborn.UIFormatters;
using Timberborn.ZiplineSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.ZiplineSystemUI;

internal class ZiplinePreviewTooltip : ILoadableSingleton
{
	private static readonly string CrossClass = "cross-red";

	private readonly ZiplineConnectionService _ziplineConnectionService;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	private VisualElement _tooltipRoot;

	private Label _distanceLabel;

	private VisualElement _distanceWarning;

	private VisualElement _distanceIcon;

	private Label _inclinationLabel;

	private VisualElement _inclinationWarning;

	private VisualElement _inclinationIcon;

	private VisualElement _warnings;

	private VisualElement _districtsWarning;

	private VisualElement _ziplineBlockedWarning;

	private VisualElement _tooManyConnectionsWarning;

	private readonly Phrase _distancePhrase = Phrase.New("Zipline.Distance").FormatDistance<int>().FormatDistance<int>();

	private readonly Phrase _inclinationPhrase = Phrase.New("Zipline.Inclination").FormatAngle<int>().FormatAngle<int>();

	public ZiplinePreviewTooltip(ZiplineConnectionService ziplineConnectionService, VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_ziplineConnectionService = ziplineConnectionService;
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public void Load()
	{
		_tooltipRoot = _visualElementLoader.LoadVisualElement("Game/ZiplineConnectionTooltip");
		_distanceLabel = _tooltipRoot.Q<Label>("Distance");
		_distanceWarning = _tooltipRoot.Q<VisualElement>("DistanceWarning");
		_distanceIcon = _tooltipRoot.Q<VisualElement>("DistanceIcon");
		_inclinationLabel = _tooltipRoot.Q<Label>("Inclination");
		_inclinationWarning = _tooltipRoot.Q<VisualElement>("InclinationWarning");
		_inclinationIcon = _tooltipRoot.Q<VisualElement>("InclinationIcon");
		_warnings = _tooltipRoot.Q<VisualElement>("WarningsWrapper");
		_districtsWarning = _tooltipRoot.Q<VisualElement>("DistrictsWarning");
		_ziplineBlockedWarning = _tooltipRoot.Q<VisualElement>("BlockedWarning");
		_tooManyConnectionsWarning = _tooltipRoot.Q<VisualElement>("TooManyConnectionsWarning");
	}

	public void ShowTooltip(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, bool isConnectable)
	{
		_tooltipRegistrar.ShowPriority(GetTooltip(ziplineTower, otherZiplineTower, isConnectable));
	}

	public void HideTooltip()
	{
		_tooltipRegistrar.HidePriority();
	}

	private VisualElement GetTooltip(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower, bool isConnectable)
	{
		bool flag = _ziplineConnectionService.DistanceIsValid(ziplineTower, otherZiplineTower, out var distance, out var maxDistance);
		bool flag2 = _ziplineConnectionService.InclinationIsValid(ziplineTower, otherZiplineTower, out var inclination, out var maxInclination);
		_distanceLabel.text = _loc.T(_distancePhrase, Mathf.CeilToInt(distance), (int)maxDistance);
		_distanceWarning.ToggleDisplayStyle(!flag);
		_distanceIcon.EnableInClassList(CrossClass, !flag);
		_inclinationLabel.text = _loc.T(_inclinationPhrase, Mathf.CeilToInt(inclination), (int)maxInclination);
		_inclinationWarning.ToggleDisplayStyle(!flag2);
		_inclinationIcon.EnableInClassList(CrossClass, !flag2);
		HideWarnings();
		if (!isConnectable & flag & flag2)
		{
			ShowWarning(ziplineTower, otherZiplineTower);
		}
		return _tooltipRoot;
	}

	private void ShowWarning(ZiplineTower ziplineTower, ZiplineTower otherZiplineTower)
	{
		_warnings.ToggleDisplayStyle(visible: true);
		if (!_ziplineConnectionService.DistrictCentersAreCompatible(ziplineTower, otherZiplineTower))
		{
			_districtsWarning.ToggleDisplayStyle(visible: true);
		}
		else if (!otherZiplineTower.HasFreeSlots)
		{
			_tooManyConnectionsWarning.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_ziplineBlockedWarning.ToggleDisplayStyle(visible: true);
		}
	}

	private void HideWarnings()
	{
		_warnings.ToggleDisplayStyle(visible: false);
		_districtsWarning.ToggleDisplayStyle(visible: false);
		_ziplineBlockedWarning.ToggleDisplayStyle(visible: false);
		_tooManyConnectionsWarning.ToggleDisplayStyle(visible: false);
	}
}
