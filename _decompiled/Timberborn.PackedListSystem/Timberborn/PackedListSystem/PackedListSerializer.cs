using System.Text;
using Timberborn.Persistence;

namespace Timberborn.PackedListSystem;

public abstract class PackedListSerializer<T> : IValueSerializer<PackedList<T>>
{
	private static readonly PropertyKey<string> ArrayKey = new PropertyKey<string>("Array");

	private static readonly char Separator = ' ';

	private readonly StringBuilder _reusableStringBuilder = new StringBuilder();

	public void Serialize(PackedList<T> packedList, IValueSaver valueSaver)
	{
		_reusableStringBuilder.Clear();
		T[] array = packedList.Array;
		int num = array.Length;
		for (int i = 0; i < num; i++)
		{
			if (i > 0)
			{
				_reusableStringBuilder.Append(Separator);
			}
			Serialize(array[i], _reusableStringBuilder);
		}
		valueSaver.AsObject().Set(ArrayKey, _reusableStringBuilder.ToString());
	}

	public Obsoletable<PackedList<T>> Deserialize(IValueLoader valueLoader)
	{
		string[] array = valueLoader.AsObject().Get(ArrayKey).Split(Separator);
		int num = array.Length;
		T[] array2 = new T[num];
		for (int i = 0; i < num; i++)
		{
			array2[i] = Deserialize(array[i]);
		}
		return new PackedList<T>(array2);
	}

	protected abstract void Serialize(T value, StringBuilder stringBuilder);

	protected abstract T Deserialize(string value);
}
