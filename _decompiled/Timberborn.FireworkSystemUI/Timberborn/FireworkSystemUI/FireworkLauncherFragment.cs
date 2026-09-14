using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.FireworkSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.FireworkSystemUI;

internal class FireworkLauncherFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly ILoc _loc;

	private VisualElement _root;

	private Dropdown _idDropdown;

	private PreciseSlider _headingSlider;

	private PreciseSlider _pitchSlider;

	private PreciseSlider _flightDistanceSlider;

	private Label _headingLabel;

	private Label _pitchLabel;

	private Label _flightDistanceLabel;

	private Toggle _isContinuousToggle;

	private FireworkLauncher _fireworkLauncher;

	private FireworkIdDropdownProvider _fireworkIdDropdownProvider;

	private readonly Phrase _headingPhrase = Phrase.New("Building.FireworkLauncher.Heading").FormatAngle<int>();

	private readonly Phrase _pitchPhrase = Phrase.New("Building.FireworkLauncher.Pitch").FormatAngle<int>();

	private readonly Phrase _flightDistancePhrase = Phrase.New("Building.FireworkLauncher.FlightDistance").FormatDistance<int>();

	public FireworkLauncherFragment(VisualElementLoader visualElementLoader, DropdownItemsSetter dropdownItemsSetter, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_dropdownItemsSetter = dropdownItemsSetter;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/FireworkLauncherFragment");
		_idDropdown = _root.Q<Dropdown>("FireworkId");
		_headingSlider = _root.Q<PreciseSlider>("HeadingSlider");
		_headingSlider.SetValueChangedCallback(OnHeadingChanged);
		_pitchSlider = _root.Q<PreciseSlider>("PitchSlider");
		_pitchSlider.SetValueChangedCallback(OnPitchChanged);
		_flightDistanceSlider = _root.Q<PreciseSlider>("FlightDistanceSlider");
		_flightDistanceSlider.SetValueChangedCallback(OnFlightDistanceChanged);
		_headingLabel = _root.Q<Label>("HeadingLabel");
		_pitchLabel = _root.Q<Label>("PitchLabel");
		_flightDistanceLabel = _root.Q<Label>("FlightDistanceLabel");
		_isContinuousToggle = _root.Q<Toggle>("IsContinuous");
		_isContinuousToggle.RegisterValueChangedCallback(OnContinuousToggleChanged);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_fireworkLauncher = entity.GetComponent<FireworkLauncher>();
		if ((bool)_fireworkLauncher)
		{
			_root.ToggleDisplayStyle(visible: true);
			_headingSlider.UpdateValuesWithoutNotify(_fireworkLauncher.Heading, FireworkLimits.MinHeading, FireworkLimits.MaxHeading);
			_pitchSlider.UpdateValuesWithoutNotify(_fireworkLauncher.Pitch, FireworkLimits.MinPitch, FireworkLimits.MaxPitch);
			_flightDistanceSlider.UpdateValuesWithoutNotify(_fireworkLauncher.FlightDistance, FireworkLimits.MinFlightDistance, FireworkLimits.MaxFlightDistance);
			_fireworkIdDropdownProvider = _fireworkLauncher.GetComponent<FireworkIdDropdownProvider>();
			_dropdownItemsSetter.SetItems(_idDropdown, _fireworkIdDropdownProvider);
		}
	}

	public void UpdateFragment()
	{
		if ((bool)_fireworkLauncher)
		{
			_isContinuousToggle.SetValueWithoutNotify(_fireworkLauncher.IsContinuous);
			UpdateLabels();
		}
	}

	public void ClearFragment()
	{
		_idDropdown.ClearItems();
		_fireworkLauncher = null;
		_fireworkIdDropdownProvider = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	private void UpdateLabels()
	{
		_headingLabel.text = _loc.T(_headingPhrase, _fireworkLauncher.Heading);
		_pitchLabel.text = _loc.T(_pitchPhrase, _fireworkLauncher.Pitch);
		_flightDistanceLabel.text = _loc.T(_flightDistancePhrase, _fireworkLauncher.FlightDistance);
	}

	private void OnHeadingChanged(float newValue)
	{
		float num = Mathf.Clamp(Mathf.Round(newValue), FireworkLimits.MinHeading, FireworkLimits.MaxHeading);
		_fireworkLauncher.SetHeading((int)num);
		UpdateLabels();
	}

	private void OnPitchChanged(float newValue)
	{
		float num = Mathf.Clamp(Mathf.Round(newValue), FireworkLimits.MinPitch, FireworkLimits.MaxPitch);
		_fireworkLauncher.SetPitch((int)num);
		UpdateLabels();
	}

	private void OnFlightDistanceChanged(float newValue)
	{
		float num = Mathf.Clamp(Mathf.Round(newValue), FireworkLimits.MinFlightDistance, FireworkLimits.MaxFlightDistance);
		_fireworkLauncher.SetFlightDistance((int)num);
		UpdateLabels();
	}

	private void OnContinuousToggleChanged(ChangeEvent<bool> evt)
	{
		_fireworkLauncher.SetContinuous(evt.newValue);
	}
}
