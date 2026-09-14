namespace Timberborn.NaturalResources;

public readonly struct SpawnableResource
{
	public string Id { get; }

	public bool IsSeedling { get; }

	public SpawnableResource(string id, bool isSeedling)
	{
		Id = id;
		IsSeedling = isSeedling;
	}
}
