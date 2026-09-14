using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DropdownSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.PlatformUtilities;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class SpeakerFragment : IEntityPanelFragment
{
	private static readonly string ModeLocKeyPrefix = "Building.Speaker.PlaybackMode.";

	private static readonly string SpatialModeLocKeyPrefix = "Building.Speaker.SpatialMode.";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly RadioToggleFactory _radioToggleFactory;

	private readonly SpeakerSoundDropdownProvider _speakerSoundDropdownProvider;

	private readonly DropdownItemsSetter _dropdownItemsSetter;

	private readonly SpeakerSoundService _speakerSoundService;

	private readonly IExplorerOpener _explorerOpener;

	private VisualElement _root;

	private RadioToggle _playbackModeRadioToggle;

	private RadioToggle _spatialModeRadioToggle;

	private Dropdown _soundId;

	private Speaker _speaker;

	public SpeakerFragment(VisualElementLoader visualElementLoader, RadioToggleFactory radioToggleFactory, SpeakerSoundDropdownProvider speakerSoundDropdownProvider, DropdownItemsSetter dropdownItemsSetter, SpeakerSoundService speakerSoundService, IExplorerOpener explorerOpener)
	{
		_visualElementLoader = visualElementLoader;
		_radioToggleFactory = radioToggleFactory;
		_speakerSoundDropdownProvider = speakerSoundDropdownProvider;
		_dropdownItemsSetter = dropdownItemsSetter;
		_speakerSoundService = speakerSoundService;
		_explorerOpener = explorerOpener;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/SpeakerFragment");
		_playbackModeRadioToggle = _radioToggleFactory.CreateLocalizable<SpeakerPlaybackMode>(ModeLocKeyPrefix, _root.Q<VisualElement>("PlaybackModeWrapper"));
		_playbackModeRadioToggle.RadioButtonSelected += OnPlaybackModeChanged;
		_spatialModeRadioToggle = _radioToggleFactory.CreateLocalizable<SpeakerSpatialMode>(SpatialModeLocKeyPrefix, _root.Q<VisualElement>("SpatialModeWrapper"));
		_spatialModeRadioToggle.RadioButtonSelected += OnSpatialModeChanged;
		_soundId = _root.Q<Dropdown>("SoundId");
		_root.Q<Button>("BrowseButton").RegisterCallback<ClickEvent>(OnBrowseButtonClicked);
		_root.Q<Button>("RefreshButton").RegisterCallback<ClickEvent>(OnRefreshButtonClicked);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		if (entity.TryGetComponent<Speaker>(out _speaker))
		{
			_speakerSoundDropdownProvider.SetSpeaker(_speaker);
			_root.ToggleDisplayStyle(visible: true);
			UpdateDropdown();
		}
	}

	public void UpdateFragment()
	{
		if ((bool)_speaker)
		{
			_playbackModeRadioToggle.Update((int)_speaker.PlaybackMode);
			_spatialModeRadioToggle.Update((int)_speaker.SpatialMode);
		}
	}

	public void ClearFragment()
	{
		_speakerSoundDropdownProvider.ClearSpeaker();
		_root.ToggleDisplayStyle(visible: false);
		_speaker = null;
	}

	private void OnPlaybackModeChanged(object sender, int index)
	{
		_speaker.SetPlaybackMode((SpeakerPlaybackMode)index);
	}

	private void OnSpatialModeChanged(object sender, int index)
	{
		_speaker.SetSpatialMode((SpeakerSpatialMode)index);
	}

	private void UpdateDropdown()
	{
		_speakerSoundDropdownProvider.UpdateSounds();
		_dropdownItemsSetter.SetItems(_soundId, _speakerSoundDropdownProvider);
	}

	private void OnBrowseButtonClicked(ClickEvent evt)
	{
		_explorerOpener.OpenDirectory(_speakerSoundService.GetCustomSoundDirectory());
	}

	private void OnRefreshButtonClicked(ClickEvent evt)
	{
		_speakerSoundService.ReloadCustomSounds();
		UpdateDropdown();
	}
}
