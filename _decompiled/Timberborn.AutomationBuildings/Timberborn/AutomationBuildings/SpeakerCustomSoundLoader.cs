using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Timberborn.FileSystem;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using UnityEngine;
using UnityEngine.Networking;

namespace Timberborn.AutomationBuildings;

internal class SpeakerCustomSoundLoader : IUnloadableSingleton
{
	private static readonly TimeSpan LoadingTimeout = TimeSpan.FromSeconds(1.0);

	private static readonly Dictionary<string, AudioType> ExtensionToAudioType = new Dictionary<string, AudioType>
	{
		{
			".wav",
			AudioType.WAV
		},
		{
			".mp3",
			AudioType.MPEG
		}
	};

	private readonly IFileService _fileService;

	private readonly AudioClipService _audioClipService;

	private readonly List<AudioClip> _loadedSounds = new List<AudioClip>();

	public SpeakerCustomSoundLoader(IFileService fileService, AudioClipService audioClipService)
	{
		_fileService = fileService;
		_audioClipService = audioClipService;
	}

	public void Unload()
	{
		UnloadCustomSounds();
	}

	public IEnumerable<AudioClip> LoadCustomSounds(string directory)
	{
		_fileService.CreateDirectory(directory);
		UnloadCustomSounds();
		LoadSounds(directory);
		return _loadedSounds;
	}

	private void LoadSounds(string directory)
	{
		foreach (string item in from path in Directory.GetFiles(directory)
			where ExtensionToAudioType.ContainsKey(GetExtension(path))
			select path)
		{
			LoadAudioFile(item);
		}
	}

	private void LoadAudioFile(string path)
	{
		try
		{
			long ticks = DateTime.Now.Ticks;
			AudioType audioType = ExtensionToAudioType[GetExtension(path)];
			using UnityWebRequest unityWebRequest = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType);
			unityWebRequest.SendWebRequest();
			while (!unityWebRequest.isDone && DateTime.Now.Ticks - ticks <= LoadingTimeout.Ticks)
			{
			}
			if (unityWebRequest.result == UnityWebRequest.Result.Success)
			{
				CreateAudioClip(unityWebRequest);
			}
			else
			{
				Debug.LogError($"Request failure when loading audio from: {path}: {unityWebRequest.result}.");
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to load audio from: " + path + ": " + ex.Message + ".");
		}
	}

	private void UnloadCustomSounds()
	{
		foreach (AudioClip loadedSound in _loadedSounds)
		{
			_audioClipService.RemoveAudioClip(loadedSound.name);
			UnityEngine.Object.Destroy(loadedSound);
		}
		_loadedSounds.Clear();
	}

	private static string GetExtension(string path)
	{
		return Path.GetExtension(path).ToLowerInvariant();
	}

	private void CreateAudioClip(UnityWebRequest request)
	{
		AudioClip content = DownloadHandlerAudioClip.GetContent(request);
		string id = (content.name = Path.GetFileName(request.url));
		_audioClipService.AddAudioClip(id, content);
		_loadedSounds.Add(content);
	}
}
