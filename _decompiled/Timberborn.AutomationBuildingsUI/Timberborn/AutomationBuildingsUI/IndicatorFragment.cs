using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class IndicatorFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private VisualElement _root;

	private Toggle _pinnedWhenOnToggle;

	private Toggle _pinnedAlwaysToggle;

	private Toggle _warningToggle;

	private Toggle _journalEntryToggle;

	private Toggle _colorReplicationToggle;

	private Indicator _indicator;

	public IndicatorFragment(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/IndicatorFragment");
		_pinnedWhenOnToggle = _root.Q<Toggle>("PinnedWhenOn");
		_pinnedAlwaysToggle = _root.Q<Toggle>("PinnedAlways");
		_warningToggle = _root.Q<Toggle>("Warning");
		_journalEntryToggle = _root.Q<Toggle>("JournalEntry");
		_colorReplicationToggle = _root.Q<Toggle>("ColorReplication");
		_pinnedWhenOnToggle.RegisterValueChangedCallback(OnPinnedWhenOnChanged);
		_pinnedAlwaysToggle.RegisterValueChangedCallback(OnPinnedAlwaysChanged);
		_warningToggle.RegisterValueChangedCallback(OnWarningToggleChanged);
		_journalEntryToggle.RegisterValueChangedCallback(OnJournalEntryToggleChanged);
		_colorReplicationToggle.RegisterValueChangedCallback(OnColorReplicationToggleChanged);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		if (entity.TryGetComponent<Indicator>(out _indicator))
		{
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void UpdateFragment()
	{
		if ((bool)_indicator)
		{
			_pinnedWhenOnToggle.SetValueWithoutNotify(_indicator.PinnedMode == IndicatorPinnedMode.WhenOn);
			_pinnedAlwaysToggle.SetValueWithoutNotify(_indicator.PinnedMode == IndicatorPinnedMode.Always);
			_warningToggle.SetValueWithoutNotify(_indicator.IsWarningEnabled);
			_journalEntryToggle.SetValueWithoutNotify(_indicator.IsJournalEntryEnabled);
			_colorReplicationToggle.SetValueWithoutNotify(_indicator.IsColorReplicationEnabled);
		}
	}

	public void ClearFragment()
	{
		_root.ToggleDisplayStyle(visible: false);
		_indicator = null;
	}

	private void OnPinnedWhenOnChanged(ChangeEvent<bool> evt)
	{
		_indicator.SetPinnedMode(evt.newValue ? IndicatorPinnedMode.WhenOn : IndicatorPinnedMode.Never);
	}

	private void OnPinnedAlwaysChanged(ChangeEvent<bool> evt)
	{
		_indicator.SetPinnedMode(evt.newValue ? IndicatorPinnedMode.Always : IndicatorPinnedMode.Never);
	}

	private void OnWarningToggleChanged(ChangeEvent<bool> evt)
	{
		_indicator.SetWarningEnabled(evt.newValue);
	}

	private void OnJournalEntryToggleChanged(ChangeEvent<bool> evt)
	{
		_indicator.SetJournalEntryEnabled(evt.newValue);
	}

	private void OnColorReplicationToggleChanged(ChangeEvent<bool> evt)
	{
		_indicator.SetColorReplicationEnabled(evt.newValue);
	}
}
