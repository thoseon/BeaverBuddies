using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace Timberborn.DropdownSystem;

public class EnumDropdownProvider<TEnum> : IExtendedDropdownProvider, IDropdownProvider where TEnum : struct
{
	private readonly Func<TEnum> _getter;

	private readonly Action<TEnum> _setter;

	private readonly Func<string, string> _displayTextGetter;

	private readonly Func<string, Sprite> _iconGetter;

	private readonly string[] _values = Enum.GetNames(typeof(TEnum));

	public IReadOnlyList<string> Items => _values;

	public EnumDropdownProvider(Func<TEnum> getter, Action<TEnum> setter, Func<string, string> displayTextGetter, Func<string, Sprite> iconGetter = null)
	{
		_getter = getter;
		_setter = setter;
		_displayTextGetter = displayTextGetter;
		_iconGetter = iconGetter;
	}

	public string GetValue()
	{
		return _getter().ToString();
	}

	public void SetValue(string value)
	{
		Enum.TryParse<TEnum>(value, out var result);
		_setter(result);
	}

	public string FormatDisplayText(string value, bool selected)
	{
		return _displayTextGetter(value);
	}

	public Sprite GetIcon(string value)
	{
		return _iconGetter?.Invoke(value);
	}

	public ImmutableArray<string> GetItemClasses(string value)
	{
		return ImmutableArray<string>.Empty;
	}
}
