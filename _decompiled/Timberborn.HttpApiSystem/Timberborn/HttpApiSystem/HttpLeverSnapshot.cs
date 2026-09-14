namespace Timberborn.HttpApiSystem;

internal readonly struct HttpLeverSnapshot
{
	public string Name { get; }

	public bool State { get; }

	public bool IsSpringReturn { get; }

	public HttpLeverSnapshot(string name, bool state, bool isSpringReturn)
	{
		Name = name;
		State = state;
		IsSpringReturn = isSpringReturn;
	}
}
