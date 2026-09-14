namespace Timberborn.NeedSystem;

public class SerializedNeed
{
	public string Id { get; }

	public float Points { get; }

	public SerializedNeed(string id, float points)
	{
		Id = id;
		Points = points;
	}
}
