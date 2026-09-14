using Timberborn.Automation;
using Timberborn.AutomationBuildings;
using Timberborn.AutomationUI;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.TimeSystem;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class TimerFragment : IEntityPanelFragment
{
	private static readonly string ModeLocKey = "Building.Timer.Mode.";

	private static readonly string ProgressFlippedClass = "timer-fragment__timer-progress--flipped";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly EnumDropdownProviderFactory _enumDropdownProviderFactory;

	private readonly TransmitterSelectorInitializer _transmitterSelectorInitializer;

	private readonly TimerIntervalElement _timerIntervalAElement;

	private readonly TimerIntervalElement _timerIntervalBElement;

	private readonly TimerModeDescriptions _timerModeDescriptions;

	private readonly ILoc _loc;

	private readonly IDayNightCycle _dayNightCycle;

	private readonly Phrase _ticksPhrase = Phrase.New().FormatTicks<int>();

	private readonly Phrase _hoursShortPhrase = Phrase.New().FormatHours<float>("F1");

	private readonly Phrase _daysShortPhrase = Phrase.New().FormatDays<float>("F1");

	private VisualElement _root;

	private Dropdown _modeDropdown;

	private EnumDropdownProvider<TimerMode> _modeDropdownProvider;

	private TransmitterSelector _inputSelector;

	private TransmitterSelector _resetInputSelector;

	private Label _modeDescription;

	private Timberborn.CoreUI.ProgressBar _timerProgressBar;

	private Label _timerProgressLabel;

	private Timer _timer;

	public TimerFragment(VisualElementLoader visualElementLoader, DropdownItemsSetter dropdownItemsSetter, EnumDropdownProviderFactory enumDropdownProviderFactory, TransmitterSelectorInitializer transmitterSelectorInitializer, TimerIntervalElement timerIntervalAElement, TimerIntervalElement timerIntervalBElement, TimerModeDescriptions timerModeDescriptions, ILoc loc, IDayNightCycle dayNightCycle)
	{
		_visualElementLoader = visualElementLoader;
		_dropdownItemsSetter = dropdownItemsSetter;
		_enumDropdownProviderFactory = enumDropdownProviderFactory;
		_transmitterSelectorInitializer = transmitterSelectorInitializer;
		_timerIntervalAElement = timerIntervalAElement;
		_timerIntervalBElement = timerIntervalBElement;
		_timerModeDescriptions = timerModeDescriptions;
		_loc = loc;
		_dayNightCycle = dayNightCycle;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/TimerFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_modeDropdown = _root.Q<Dropdown>("Mode");
		_modeDropdownProvider = _enumDropdownProviderFactory.CreateLocalized(() => _timer.Mode, delegate(TimerMode mode)
		{
			_timer.SetMode(mode);
		}, ModeLocKey);
		_inputSelector = _root.Q<TransmitterSelector>("Input");
		_transmitterSelectorInitializer.Initialize(_inputSelector, () => _timer.Input, delegate(Automator automator)
		{
			_timer.SetInput(automator);
		});
		_resetInputSelector = _root.Q<TransmitterSelector>("ResetInput");
		_transmitterSelectorInitializer.InitializeOptional(_resetInputSelector, () => _timer.ResetInput, delegate(Automator automator)
		{
			_timer.SetResetInput(automator);
		});
		_timerIntervalAElement.Initialize(_root.Q<VisualElement>("TimerIntervalA"));
		_timerIntervalBElement.Initialize(_root.Q<VisualElement>("TimerIntervalB"));
		_modeDescription = _root.Q<Label>("ModeDescription");
		_timerProgressBar = _root.Q<Timberborn.CoreUI.ProgressBar>("TimerProgressBar");
		_timerProgressLabel = _root.Q<Label>("TimerProgressLabel");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		if (entity.TryGetComponent<Timer>(out _timer))
		{
			_dropdownItemsSetter.SetItems(_modeDropdown, _modeDropdownProvider);
			_inputSelector.Show(_timer);
			_resetInputSelector.Show(_timer);
			_timerIntervalAElement.Show(_timer.TimerIntervalA);
			_timerIntervalBElement.Show(_timer.TimerIntervalB);
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void UpdateFragment()
	{
		if ((bool)_timer)
		{
			_inputSelector.UpdateStateIcon();
			_resetInputSelector.UpdateStateIcon();
			_timerIntervalAElement.Update();
			if (_timer.UsesIntervalB)
			{
				_timerIntervalBElement.Update();
				_timerIntervalBElement.SetDisplayStyle(visible: true);
			}
			else
			{
				_timerIntervalBElement.SetDisplayStyle(visible: false);
			}
			UpdateTimerProgress();
			_modeDescription.text = _timerModeDescriptions.GetDescription(_timer.Mode);
		}
	}

	public void ClearFragment()
	{
		_modeDropdown.ClearItems();
		_inputSelector.ClearItems();
		_resetInputSelector.ClearItems();
		_timerIntervalAElement.Clear();
		_timerIntervalBElement.Clear();
		_timer = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	private void UpdateTimerProgress()
	{
		float progress = _timer.GetProgress(out var isCountingTimeB);
		float progress2 = (isCountingTimeB ? (1f - progress) : progress);
		_timerProgressBar.SetProgress(progress2);
		_timerProgressBar.EnableInClassList(ProgressFlippedClass, isCountingTimeB);
		int ticksLeft = _timer.GetTicksLeft();
		if (_timer.IsUsingTicks())
		{
			_timerProgressLabel.text = _loc.T(_ticksPhrase, ticksLeft);
			return;
		}
		float num = _dayNightCycle.TicksToHours(ticksLeft);
		_timerProgressLabel.text = ((num > 24f) ? _loc.T(_daysShortPhrase, num / 24f) : _loc.T(_hoursShortPhrase, num));
	}
}
