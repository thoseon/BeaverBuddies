namespace Timberborn.HttpApiSystem;

internal readonly struct HttpAdapterSnapshot
{
	public string Name { get; }

	public bool State { get; }

	public HttpAdapterSnapshot(string name, bool state)
	{
		Name = name;
		State = state;
	}
}
