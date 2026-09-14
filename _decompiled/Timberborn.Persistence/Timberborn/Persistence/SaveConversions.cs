using System.Collections.Generic;

namespace Timberborn.Persistence;

public abstract class SaveConversions
{
	public static object Convert<T>(T value, IValueSerializer<T> serializer)
	{
		ValueSaver valueSaver = new ValueSaver();
		serializer.Serialize(value, valueSaver);
		return valueSaver.Value;
	}

	public static object[] ConvertList<T>(IReadOnlyCollection<T> values, IValueSerializer<T> serializer)
	{
		object[] array = new object[values.Count];
		int num = 0;
		foreach (T value in values)
		{
			array[num++] = Convert(value, serializer);
		}
		return array;
	}

	public static bool Deconvert<T>(object inputValue, IValueSerializer<T> serializer, out T value)
	{
		ValueLoader valueLoader = new ValueLoader(inputValue);
		Obsoletable<T> obsoletable = serializer.Deserialize(valueLoader);
		value = (obsoletable.Obsolete ? default(T) : obsoletable.Value);
		return !obsoletable.Obsolete;
	}

	public static List<T> DeconvertList<T>(IValueSerializer<T> serializer, IReadOnlyList<object> inputValues)
	{
		List<T> list = new List<T>(inputValues.Count);
		for (int i = 0; i < inputValues.Count; i++)
		{
			if (Deconvert(inputValues[i], serializer, out var value))
			{
				list.Add(value);
			}
		}
		return list;
	}
}
