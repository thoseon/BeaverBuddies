namespace Timberborn.BlockObjectModelSystem;

public interface IBlockObjectModel
{
	bool HasUndergroundModel { get; }

	int UndergroundModelDepth { get; }

	bool HasUncoveredModel { get; }

	bool UnfinishedConstructionModeModel { get; }

	void UpdateModelVisibility();
}
