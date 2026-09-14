namespace Timberborn.Effects;

public readonly struct Effect
{
	public float Points { get; }

	public int Count { get; }

	private Effect(float points, int count)
	{
		Points = points;
		Count = count;
	}

	public static Effect From(InstantEffect instantEffect)
	{
		return new Effect(instantEffect.Points, instantEffect.Count);
	}
}
