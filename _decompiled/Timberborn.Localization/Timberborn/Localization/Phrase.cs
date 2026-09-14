using System;

namespace Timberborn.Localization;

public class Phrase
{
	private TextLocalizationWrapper _textLocalizationWrapper;

	private readonly int _customFormatters;

	private readonly object _formatter1;

	private readonly object _formatter2;

	private readonly object _formatter3;

	internal string Key { get; }

	private Phrase(string key)
	{
		Key = key;
	}

	private Phrase(string key, object formatter1)
	{
		Key = key;
		_formatter1 = formatter1;
		_customFormatters = 1;
	}

	private Phrase(string key, object formatter1, object formatter2)
	{
		ValidateKeyExists(key);
		Key = key;
		_formatter1 = formatter1;
		_formatter2 = formatter2;
		_customFormatters = 2;
	}

	private Phrase(string key, object formatter1, object formatter2, object formatter3)
	{
		ValidateKeyExists(key);
		Key = key;
		_formatter1 = formatter1;
		_formatter2 = formatter2;
		_formatter3 = formatter3;
		_customFormatters = 3;
	}

	public static Phrase New()
	{
		return new Phrase(null);
	}

	public static Phrase New(string key)
	{
		return new Phrase(key);
	}

	public Phrase Format()
	{
		return Format((Func<object, ILoc, string>)null);
	}

	public Phrase Format<T>(string format) where T : IFormattable
	{
		return Format((T value) => value.ToString(format, null));
	}

	public Phrase Format<T>(Func<T, string> formatter)
	{
		return Format((T value, ILoc _) => formatter(value));
	}

	public Phrase Format<T>(Func<T, ILoc, string> formatter)
	{
		return _customFormatters switch
		{
			0 => new Phrase(Key, formatter), 
			1 => new Phrase(Key, _formatter1, formatter), 
			2 => new Phrase(Key, _formatter1, _formatter2, formatter), 
			_ => throw new InvalidOperationException("Trying to add too many formatters."), 
		};
	}

	internal string GetText<TP1, TP2, TP3>(ILoc loc, TP1 value1, TP2 value2, TP3 value3)
	{
		if (_textLocalizationWrapper == null)
		{
			_textLocalizationWrapper = new TextLocalizationWrapper((Key == null) ? null : loc.T(Key));
		}
		return _textLocalizationWrapper.GetText(loc, value1, value2, value3, _formatter1, _formatter2, _formatter3);
	}

	private static void ValidateKeyExists(string key)
	{
		if (key == null)
		{
			throw new InvalidOperationException("A non-localized Phrase can only have one formatter.");
		}
	}
}
