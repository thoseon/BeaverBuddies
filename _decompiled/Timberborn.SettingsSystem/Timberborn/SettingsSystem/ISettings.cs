using System.Collections.Immutable;

namespace Timberborn.SettingsSystem;

public interface ISettings
{
	int GetInt(string key, int defaultValue);

	int GetSafeInt(string key, int defaultValue);

	void SetInt(string key, int value);

	bool GetBool(string key, bool defaultValue = false);

	bool GetSafeBool(string key, bool defaultValue = false);

	void SetBool(string key, bool value);

	float GetFloat(string key, float defaultValue);

	float GetSafeFloat(string key, float defaultValue);

	void SetFloat(string key, float value);

	string GetString(string key, string defaultValue);

	string GetSafeString(string key, string defaultValue);

	void SetString(string key, string value);

	bool Has(string key);

	void Clear(string key);

	void ValidateInt(string key, ImmutableArray<int> validValues, int defaultValue);

	void ValidateString(string key, ImmutableArray<string> validValues, string defaultValue);
}
