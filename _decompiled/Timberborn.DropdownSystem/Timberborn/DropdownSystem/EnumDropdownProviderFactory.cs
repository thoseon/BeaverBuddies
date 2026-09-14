using System;
using Timberborn.Localization;
using UnityEngine;

namespace Timberborn.DropdownSystem;

public class EnumDropdownProviderFactory
{
	private readonly ILoc _loc;

	public EnumDropdownProviderFactory(ILoc loc)
	{
		_loc = loc;
	}

	public EnumDropdownProvider<TEnum> Create<TEnum>(Func<TEnum> getter, Action<TEnum> setter, Func<string, string> displayText) where TEnum : struct
	{
		return new EnumDropdownProvider<TEnum>(getter, setter, displayText);
	}

	public EnumDropdownProvider<TEnum> CreateWithIcon<TEnum>(Func<TEnum> getter, Action<TEnum> setter, Func<string, string> displayText, Func<string, Sprite> iconGetter) where TEnum : struct
	{
		return new EnumDropdownProvider<TEnum>(getter, setter, displayText, iconGetter);
	}

	public EnumDropdownProvider<TEnum> CreateLocalized<TEnum>(Func<TEnum> getter, Action<TEnum> setter, string valueLocKeyPrefix) where TEnum : struct
	{
		return new EnumDropdownProvider<TEnum>(getter, setter, (string value) => GetLocalizedValue(valueLocKeyPrefix, value));
	}

	private string GetLocalizedValue(string prefix, string value)
	{
		return _loc.T(prefix + value);
	}
}
