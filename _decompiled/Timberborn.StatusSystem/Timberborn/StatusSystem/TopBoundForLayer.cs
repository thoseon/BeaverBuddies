namespace Timberborn.StatusSystem;

internal readonly struct TopBoundForLayer
{
	public float ConstructionModeTopBound { get; }

	public float NormalModeTopBound { get; }

	public TopBoundForLayer(float constructionModeTopBound)
		: this(constructionModeTopBound, constructionModeTopBound)
	{
	}

	public TopBoundForLayer(float constructionModeTopBound, float normalModeTopBound)
	{
		ConstructionModeTopBound = constructionModeTopBound;
		NormalModeTopBound = normalModeTopBound;
	}
}
