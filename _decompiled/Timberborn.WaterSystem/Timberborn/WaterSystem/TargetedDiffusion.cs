namespace Timberborn.WaterSystem;

internal readonly struct TargetedDiffusion(int targetIndex3D, int originIndex3D)
{
	public readonly int TargetIndex3D = targetIndex3D;

	public readonly int OriginIndex3D = originIndex3D;
}
