namespace Timberborn.WorldPersistence;

public readonly struct ComponentKey
{
	public string Name { get; }

	public ComponentKey(string name)
	{
		Name = name;
	}

	public ComponentKey AddSuffix(string suffix)
	{
		return new ComponentKey(Name + ":" + suffix);
	}
}
