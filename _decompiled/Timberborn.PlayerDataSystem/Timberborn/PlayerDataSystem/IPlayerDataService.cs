namespace Timberborn.PlayerDataSystem;

public interface IPlayerDataService
{
	bool DataLoadSuccessful { get; }

	bool HasKey(string key);

	bool GetBool(string key, bool defaultValue);

	string GetString(string key, string defaultValue);

	void SetBool(string key, bool value);

	void SetString(string key, string value);

	void Remove(string key);

	void RemoveAll();
}
