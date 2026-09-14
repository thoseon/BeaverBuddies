using Timberborn.BlueprintSystem;

namespace Timberborn.CoreSound;

internal record CoreSoundSpec : ComponentSpec
{
	[Serialize]
	public int MaxVerticalListenerPositionAboveGround { get; init; }

	[Serialize]
	public float MinBuildingFadeDistance { get; init; }

	[Serialize]
	public float MaxBuildingFadeDistance { get; init; }

	[Serialize]
	public string WindAmbientKey { get; init; }

	[Serialize]
	public float MinAmbientFade { get; init; }

	[Serialize]
	public float MaxAmbientFade { get; init; }

	[Serialize]
	public float MinWindFade { get; init; }

	[Serialize]
	public float MaxWindFade { get; init; }
}
