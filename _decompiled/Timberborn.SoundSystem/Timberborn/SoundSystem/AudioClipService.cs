using System.Collections.Generic;
using System.Linq;
using Timberborn.AssetSystem;
using UnityEngine;

namespace Timberborn.SoundSystem;

public class AudioClipService
{
	private static readonly string SoundsDirectoryKey = "Sounds";

	private readonly IAssetLoader _assetLoader;

	private readonly Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

	public AudioClipService(IAssetLoader assetLoader)
	{
		_assetLoader = assetLoader;
	}

	public void LoadAudioClips()
	{
		foreach (LoadedAsset<AudioClip> item in _assetLoader.LoadAll<AudioClip>(SoundsDirectoryKey))
		{
			_audioClips[item.Asset.name] = item.Asset;
		}
	}

	public void AddAudioClip(string id, AudioClip audioClip)
	{
		_audioClips[id] = audioClip;
	}

	public void RemoveAudioClip(string id)
	{
		_audioClips.Remove(id);
	}

	public AudioClip GetAudioClip(string soundName)
	{
		return _audioClips[soundName];
	}

	public IEnumerable<string> GetAudioClipNames(string soundName)
	{
		string appendedName = soundName + "_";
		return _audioClips.Keys.Where((string name) => name == soundName || name.StartsWith(appendedName));
	}
}
