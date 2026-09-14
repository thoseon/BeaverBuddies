using System.Collections.Generic;

namespace Timberborn.Localization;

internal class TextLocalization<T1, T2, T3>
{
	private T1 _value1;

	private T2 _value2;

	private T3 _value3;

	public string Text { get; private set; }

	public TextLocalization(T1 value1, T2 value2, T3 value3, string text)
	{
		_value1 = value1;
		_value2 = value2;
		_value3 = value3;
		Text = text;
	}

	public void Update(T1 value1, T2 value2, T3 value3, string text)
	{
		_value1 = value1;
		_value2 = value2;
		_value3 = value3;
		Text = text;
	}

	public bool AreValuesEqual(T1 value1, T2 value2, T3 value3)
	{
		if (EqualityComparer<T1>.Default.Equals(_value1, value1) && EqualityComparer<T2>.Default.Equals(_value2, value2))
		{
			return EqualityComparer<T3>.Default.Equals(_value3, value3);
		}
		return false;
	}
}
