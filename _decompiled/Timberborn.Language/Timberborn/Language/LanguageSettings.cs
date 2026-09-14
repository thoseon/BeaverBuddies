using Timberborn.SettingsSystem;
using Timberborn.StoreSystem;

namespace Timberborn.Language;

public class LanguageSettings
{
	public static readonly string LanguageKey = "CurrentLanguage";

	private readonly IStore _store;

	private readonly ISettings _settings;

	public string Language
	{
		get
		{
			return _settings.GetSafeString(LanguageKey, _store.Language);
		}
		set
		{
			_settings.SetString(LanguageKey, value);
		}
	}

	public LanguageSettings(IStore store, ISettings settings)
	{
		_store = store;
		_settings = settings;
	}
}
