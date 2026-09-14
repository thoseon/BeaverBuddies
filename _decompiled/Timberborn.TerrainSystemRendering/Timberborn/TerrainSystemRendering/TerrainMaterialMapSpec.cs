using Timberborn.BlueprintSystem;
using UnityEngine;

namespace Timberborn.TerrainSystemRendering;

internal record TerrainMaterialMapSpec : ComponentSpec
{
	[Serialize]
	public AssetRef<Texture2D> DesertTexture { get; init; }

	[Serialize]
	public AssetRef<Texture2D> DryFieldTexture { get; init; }

	[Serialize]
	public AssetRef<Texture2D> WetFieldTexture { get; init; }

	[Serialize]
	public AssetRef<Texture2D> BlendingNoise { get; init; }

	[Serialize]
	public float BlendingNoiseScale { get; init; }

	[Serialize]
	public float BlendingNoiseMultiplier { get; init; }

	[Serialize]
	public float BlendingSoftness { get; init; }

	[Serialize]
	public float BlendingMargin { get; init; }

	[Serialize]
	public float AltitudeCeiling { get; init; }

	[Serialize]
	public AssetRef<Texture> AltitudeMultiplier { get; init; }

	[Serialize]
	public AssetRef<Texture> DesertAltitudeMultiplier { get; init; }

	[Serialize]
	public float CutoutMargin { get; init; }
}
