using System;
using UnityEngine;

namespace Timberborn.Localization;

internal class TextLocalizationWrapper
{
	private readonly string _textTemplate;

	private object _textLocalization;

	public TextLocalizationWrapper(string textTemplate)
	{
		_textTemplate = textTemplate;
	}

	public string GetText<TP1, TP2, TP3>(ILoc loc, TP1 value1, TP2 value2, TP3 value3)
	{
		return GetText(loc, value1, value2, value3, null, null, null);
	}

	public string GetText<TP1, TP2, TP3>(ILoc loc, TP1 value1, TP2 value2, TP3 value3, object formatter1, object formatter2, object formatter3)
	{
		if (_textLocalization is TextLocalization<TP1, TP2, TP3> textLocalization)
		{
			return GetText(loc, textLocalization, value1, value2, value3, formatter1, formatter2, formatter3);
		}
		return CreateNewTextLocalization(loc, value1, value2, value3, formatter1, formatter2, formatter3);
	}

	private string GetText<TP1, TP2, TP3>(ILoc loc, TextLocalization<TP1, TP2, TP3> textLocalization, TP1 value1, TP2 value2, TP3 value3, object formatter1, object formatter2, object formatter3)
	{
		if (!textLocalization.AreValuesEqual(value1, value2, value3))
		{
			string text = Format(loc, value1, value2, value3, formatter1, formatter2, formatter3);
			textLocalization.Update(value1, value2, value3, text);
		}
		return textLocalization.Text;
	}

	private string CreateNewTextLocalization<TP1, TP2, TP3>(ILoc loc, TP1 value1, TP2 value2, TP3 value3, object formatter1, object formatter2, object formatter3)
	{
		WarnIfUpdating(value1, value2, value3);
		string text = Format(loc, value1, value2, value3, formatter1, formatter2, formatter3);
		_textLocalization = new TextLocalization<TP1, TP2, TP3>(value1, value2, value3, text);
		return text;
	}

	private void WarnIfUpdating<TP1, TP2, TP3>(TP1 value1, TP2 value2, TP3 value3)
	{
		if (_textLocalization != null)
		{
			Debug.LogWarning("TextLocalizationWrapper parameter types have changed. This shouldn't have happened!" + $" Current types: {value1?.GetType()} {value2?.GetType()} {value3?.GetType()}");
		}
	}

	private string Format<TP1, TP2, TP3>(ILoc loc, TP1 value1, TP2 value2, TP3 value3, object formatter1, object formatter2, object formatter3)
	{
		if (_textTemplate == null)
		{
			return Format(loc, value1, formatter1)?.ToString() ?? "";
		}
		return string.Format(_textTemplate, Format(loc, value1, formatter1), Format(loc, value2, formatter2), Format(loc, value3, formatter3));
	}

	private static object Format<T>(ILoc loc, T value, object formatter)
	{
		if (formatter != null)
		{
			if (formatter is Func<T, ILoc, string> func)
			{
				return func(value, loc);
			}
			return FormatFallback(value, formatter);
		}
		return value;
	}

	private static object FormatFallback<T>(T value, object formatter)
	{
		Debug.LogWarning($"Argument type {typeof(T)}" + $" does not match formatter type {formatter.GetType()}.");
		return value;
	}
}
