using System.Collections.Generic;
using UnityEngine;

namespace Timberborn.Localization;

internal class Loc : ILoc
{
	private Dictionary<string, string> _localization;

	private readonly Dictionary<string, TextLocalizationWrapper> _localizationCache = new Dictionary<string, TextLocalizationWrapper>();

	public void Initialize(Dictionary<string, string> localization)
	{
		_localizationCache.Clear();
		_localization = localization;
	}

	public IEnumerable<string> GetRawTexts()
	{
		return _localization.Values;
	}

	public string T(string key)
	{
		if (key != null && _localization.TryGetValue(key, out var value))
		{
			return value;
		}
		Debug.LogError("No localization for " + key);
		return key;
	}

	public string T<T1>(string key, T1 param1)
	{
		return T<T1, object, object>(key, param1, null, null);
	}

	public string T<T1, T2>(string key, T1 param1, T2 param2)
	{
		return T<T1, T2, object>(key, param1, param2, null);
	}

	public string T<T1, T2, T3>(string key, T1 param1, T2 param2, T3 param3)
	{
		if (_localizationCache.TryGetValue(key, out var value))
		{
			return value.GetText(this, param1, param2, param3);
		}
		TextLocalizationWrapper textLocalizationWrapper = new TextLocalizationWrapper(T(key));
		_localizationCache.Add(key, textLocalizationWrapper);
		return textLocalizationWrapper.GetText(this, param1, param2, param3);
	}

	public string T(Phrase phrase)
	{
		return T(phrase.Key);
	}

	public string T<T1>(Phrase phrase, T1 param1)
	{
		return phrase.GetText<T1, object, object>(this, param1, null, null);
	}

	public string T<T1, T2>(Phrase phrase, T1 param1, T2 param2)
	{
		return phrase.GetText<T1, T2, object>(this, param1, param2, null);
	}

	public string T<T1, T2, T3>(Phrase phrase, T1 param1, T2 param2, T3 param3)
	{
		return phrase.GetText(this, param1, param2, param3);
	}
}
