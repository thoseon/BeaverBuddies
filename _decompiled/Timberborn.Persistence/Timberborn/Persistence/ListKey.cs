namespace Timberborn.Persistence;

public readonly struct ListKey<T>
{
	public string Name { get; }

	public ListKey(string name)
	{
		Name = name;
	}
}
