using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.SingletonSystem;

namespace Timberborn.PlayerDataSystem;

internal class PlayerDataService : ILoadableSingleton, IPlayerDataService
{
	private readonly PlayerDataSerializer _playerDataSerializer;

	private readonly PlayerDataFileService _playerDataFileService;

	private Dictionary<string, string> _playerData;

	public bool DataLoadSuccessful { get; private set; }

	public PlayerDataService(PlayerDataSerializer playerDataSerializer, PlayerDataFileService playerDataFileService)
	{
		_playerDataSerializer = playerDataSerializer;
		_playerDataFileService = playerDataFileService;
	}

	public void Load()
	{
		_playerData = _playerDataSerializer.LoadData(out var success);
		DataLoadSuccessful = success;
		if (DataLoadSuccessful)
		{
			_playerDataFileService.BackupFile();
			return;
		}
		string text = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd-HH\\hmm\\mss\\s");
		_playerDataFileService.CopyFile("corrupted." + text);
		_playerDataFileService.RestoreFromBackup();
		_playerData = _playerDataSerializer.LoadData(out var _);
	}

	public bool HasKey(string key)
	{
		return _playerData.ContainsKey(key);
	}

	public bool GetBool(string key, bool defaultValue)
	{
		if (!bool.TryParse(_playerData.GetOrDefault(key), out var result))
		{
			return defaultValue;
		}
		return result;
	}

	public string GetString(string key, string defaultValue)
	{
		return _playerData.GetOrDefault(key) ?? defaultValue;
	}

	public void SetBool(string key, bool value)
	{
		Set(key, value.ToString());
	}

	public void SetString(string key, string value)
	{
		Set(key, value);
	}

	public void Remove(string key)
	{
		_playerData.Remove(key);
		Save();
	}

	public void RemoveAll()
	{
		_playerData.Clear();
		Save();
	}

	private void Set(string key, string value)
	{
		_playerData[key] = value;
		Save();
	}

	private void Save()
	{
		_playerDataSerializer.SaveData(_playerData);
	}
}
