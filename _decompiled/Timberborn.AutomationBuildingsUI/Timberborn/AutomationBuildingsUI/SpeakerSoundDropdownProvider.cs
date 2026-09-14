using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Timberborn.AssetSystem;
using Timberborn.AutomationBuildings;
using Timberborn.Common;
using Timberborn.DropdownSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.AutomationBuildingsUI;

internal class SpeakerSoundDropdownProvider : ILoadableSingleton, IExtendedDropdownProvider, IDropdownProvider
{
	private readonly SpeakerSoundService _speakerSoundService;

	private readonly IAssetLoader _assetLoader;

	private readonly List<string> _sounds = new List<string>();

	private Speaker _speaker;

	private int _selectedIndex;

	private Sprite _buildInSoundIcon;

	private Sprite _customSoundIcon;

	public IReadOnlyList<string> Items => _sounds.AsReadOnlyList();

	public SpeakerSoundDropdownProvider(SpeakerSoundService speakerSoundService, IAssetLoader assetLoader)
	{
		_speakerSoundService = speakerSoundService;
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		_buildInSoundIcon = _assetLoader.Load<Sprite>("UI/Images/Game/sound-builtin");
		_customSoundIcon = _assetLoader.Load<Sprite>("UI/Images/Game/sound-custom");
	}

	public void UpdateSounds()
	{
		_sounds.Clear();
		_sounds.AddRange(_speakerSoundService.BuiltInSounds);
		_sounds.AddRange(_speakerSoundService.CustomSounds.OrderBy((string sound) => sound));
	}

	public void SetSpeaker(Speaker speaker)
	{
		_speaker = speaker;
	}

	public void ClearSpeaker()
	{
		_speaker = null;
	}

	public string GetValue()
	{
		return _speaker.SoundId;
	}

	public void SetValue(string value)
	{
		_speaker.SetSoundId(value);
	}

	public string FormatDisplayText(string value, bool selected)
	{
		if (!_speakerSoundService.BuiltInSounds.Contains(value))
		{
			return value;
		}
		return _speakerSoundService.GetSoundDisplayName(value);
	}

	public Sprite GetIcon(string value)
	{
		if (!_speakerSoundService.BuiltInSounds.Contains(value))
		{
			return _customSoundIcon;
		}
		return _buildInSoundIcon;
	}

	public ImmutableArray<string> GetItemClasses(string value)
	{
		return ImmutableArray<string>.Empty;
	}
}
