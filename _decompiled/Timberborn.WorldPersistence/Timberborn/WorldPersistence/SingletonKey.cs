namespace Timberborn.WorldPersistence;

public readonly struct SingletonKey
{
	public string Name { get; }

	public SingletonKey(string name)
	{
		Name = name;
	}
}
