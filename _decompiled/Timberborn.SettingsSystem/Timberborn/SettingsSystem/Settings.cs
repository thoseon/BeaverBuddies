using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.CommandLine;
using UnityEngine;

namespace Timberborn.SettingsSystem;

internal class Settings : ISettings
{
	private static readonly string SafeModeKey = "safe";

	private readonly ICommandLineArguments _commandLineArguments;

	private readonly HashSet<string> _touchedSafeKeys = new HashSet<string>();

	public Settings(ICommandLineArguments commandLineArguments)
	{
		_commandLineArguments = commandLineArguments;
	}

	public int GetInt(string key, int defaultValue)
	{
		return PlayerPrefs.GetInt(key, defaultValue);
	}

	public int GetSafeInt(string key, int defaultValue)
	{
		DeleteIfSafeMode(key);
		return GetInt(key, defaultValue);
	}

	public void SetInt(string key, int value)
	{
		_touchedSafeKeys.Add(key);
		PlayerPrefs.SetInt(key, value);
	}

	public bool GetBool(string key, bool defaultValue = false)
	{
		int defaultValue2 = (defaultValue ? 1 : 0);
		return PlayerPrefs.GetInt(key, defaultValue2) == 1;
	}

	public bool GetSafeBool(string key, bool defaultValue = false)
	{
		DeleteIfSafeMode(key);
		return GetBool(key, defaultValue);
	}

	public void SetBool(string key, bool value)
	{
		_touchedSafeKeys.Add(key);
		PlayerPrefs.SetInt(key, value ? 1 : 0);
	}

	public float GetFloat(string key, float defaultValue)
	{
		return PlayerPrefs.GetFloat(key, defaultValue);
	}

	public float GetSafeFloat(string key, float defaultValue)
	{
		DeleteIfSafeMode(key);
		return GetFloat(key, defaultValue);
	}

	public void SetFloat(string key, float value)
	{
		_touchedSafeKeys.Add(key);
		PlayerPrefs.SetFloat(key, value);
	}

	public string GetString(string key, string defaultValue)
	{
		return PlayerPrefs.GetString(key, defaultValue);
	}

	public string GetSafeString(string key, string defaultValue)
	{
		DeleteIfSafeMode(key);
		return GetString(key, defaultValue);
	}

	public void SetString(string key, string value)
	{
		_touchedSafeKeys.Add(key);
		PlayerPrefs.SetString(key, value);
	}

	public bool Has(string key)
	{
		return PlayerPrefs.HasKey(key);
	}

	public void Clear(string key)
	{
		PlayerPrefs.DeleteKey(key);
	}

	public void ValidateInt(string key, ImmutableArray<int> validValues, int defaultValue)
	{
		int num = GetInt(key, defaultValue);
		if (validValues.IndexOf(num) == -1)
		{
			Debug.LogWarning($"Invalid setting value for key\"{key}\": {num}. Changing to {defaultValue}");
			SetInt(key, defaultValue);
		}
	}

	public void ValidateString(string key, ImmutableArray<string> validValues, string defaultValue)
	{
		string safeString = GetSafeString(key, defaultValue);
		if (validValues.IndexOf(safeString) == -1)
		{
			Debug.LogWarning("Invalid setting value for key\"" + key + "\": " + safeString + ". Changing to " + defaultValue);
			SetString(key, defaultValue);
		}
	}

	private void DeleteIfSafeMode(string key)
	{
		if (_commandLineArguments.Has(SafeModeKey) && _touchedSafeKeys.Add(key))
		{
			Clear(key);
		}
	}
}
