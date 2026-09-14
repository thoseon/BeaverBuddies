namespace Timberborn.Persistence;

public readonly struct PropertyKey<T>
{
	public string Name { get; }

	public PropertyKey(string name)
	{
		Name = name;
	}
}
