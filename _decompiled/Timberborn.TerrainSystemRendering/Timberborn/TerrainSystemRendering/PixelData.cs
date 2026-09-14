using JetBrains.Annotations;

namespace Timberborn.TerrainSystemRendering;

public readonly struct PixelData
{
	[UsedImplicitly]
	private byte R { get; }

	[UsedImplicitly]
	private byte G { get; }

	public float GNormalized => (float)(int)G / 255f;

	public PixelData(float r, float g)
	{
		R = (byte)(255f * r);
		G = (byte)(255f * g);
	}
}
