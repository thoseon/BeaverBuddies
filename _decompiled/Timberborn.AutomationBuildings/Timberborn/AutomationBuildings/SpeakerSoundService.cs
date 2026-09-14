using System.Collections.Generic;
using System.IO;
using System.Linq;
using Timberborn.Common;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

public class SpeakerSoundService : ILoadableSingleton
{
	private static readonly string FolderName = "Sounds";

	private readonly SpeakerBuiltinSounds _speakerBuiltinSounds;

	private readonly SpeakerCustomSoundLoader _speakerCustomSoundLoader;

	private readonly EventBus _eventBus;

	private readonly List<string> _customSoundIds = new List<string>();

	public ReadOnlyList<string> BuiltInSounds => _speakerBuiltinSounds.SoundIds;

	public ReadOnlyList<string> CustomSounds => _customSoundIds.AsReadOnlyList();

	internal SpeakerSoundService(SpeakerBuiltinSounds speakerBuiltinSounds, SpeakerCustomSoundLoader speakerCustomSoundLoader, EventBus eventBus)
	{
		_speakerBuiltinSounds = speakerBuiltinSounds;
		_speakerCustomSoundLoader = speakerCustomSoundLoader;
		_eventBus = eventBus;
	}

	public void Load()
	{
		LoadCustomSounds();
	}

	public string GetValidatedSoundId(string soundName)
	{
		if (CustomSounds.Contains(soundName) || BuiltInSounds.Contains(soundName))
		{
			return soundName;
		}
		return BuiltInSounds.First();
	}

	public void ReloadCustomSounds()
	{
		_customSoundIds.Clear();
		LoadCustomSounds();
		_eventBus.Post(new SpeakerSoundsReloadedEvent());
	}

	public string GetCustomSoundDirectory()
	{
		return Path.Combine(UserDataFolder.Folder, FolderName);
	}

	public string GetSoundDisplayName(string soundId)
	{
		return _speakerBuiltinSounds.GetSoundDisplayName(soundId);
	}

	private void LoadCustomSounds()
	{
		string customSoundDirectory = GetCustomSoundDirectory();
		foreach (AudioClip item in _speakerCustomSoundLoader.LoadCustomSounds(customSoundDirectory))
		{
			_customSoundIds.Add(item.name);
		}
	}
}
