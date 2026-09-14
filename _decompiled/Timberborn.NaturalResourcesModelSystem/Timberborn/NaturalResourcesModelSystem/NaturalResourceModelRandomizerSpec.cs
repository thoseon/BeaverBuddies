using Timberborn.BlueprintSystem;

namespace Timberborn.NaturalResourcesModelSystem;

internal record NaturalResourceModelRandomizerSpec : ComponentSpec
{
	[Serialize]
	public bool ConstrainProportion { get; init; }

	[Serialize]
	public float MinHeightScaleFactor { get; init; } = 0.8f;

	[Serialize]
	public float MaxHeightScaleFactor { get; init; } = 1.2f;

	[Serialize]
	public float MinWidthScaleFactor { get; init; } = 0.8f;

	[Serialize]
	public float MaxWidthScaleFactor { get; init; } = 1.2f;

	[Serialize]
	public RandomizeRotationMode RandomizedRotation { get; init; }

	[Serialize]
	public float MinRotation { get; init; }

	[Serialize]
	public float MaxRotation { get; init; } = 360f;
}
