namespace Timberborn.Persistence;

public interface IValueSerializer<T>
{
	void Serialize(T value, IValueSaver valueSaver);

	Obsoletable<T> Deserialize(IValueLoader valueLoader);
}
