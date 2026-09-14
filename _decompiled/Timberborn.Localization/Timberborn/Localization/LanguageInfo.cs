namespace Timberborn.Localization;

public readonly struct LanguageInfo
{
	public string LocalizationCode { get; }

	public string DisplayName { get; }

	public bool IsNew { get; }

	public LanguageInfo(string localizationCode, string displayName, bool isNew)
	{
		LocalizationCode = localizationCode;
		DisplayName = displayName;
		IsNew = isNew;
	}
}
