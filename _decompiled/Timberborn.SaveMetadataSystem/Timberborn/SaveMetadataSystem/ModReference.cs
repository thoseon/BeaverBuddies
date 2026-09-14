namespace Timberborn.SaveMetadataSystem;

public readonly struct ModReference
{
	public string Id { get; }

	public string Name { get; }

	public string Version { get; }

	public ModReference(string id, string name, string version)
	{
		Id = id;
		Name = name;
		Version = version;
	}
}
