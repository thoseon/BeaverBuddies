namespace Timberborn.WaterSystem;

internal readonly struct DirectedFlow(float flow, int targetIndex3D, int originIndex3D)
{
	public readonly float Flow = flow;

	public readonly int TargetIndex3D = targetIndex3D;

	public readonly int OriginIndex3D = originIndex3D;

	public DirectedFlow MultiplyFlow(float modifer)
	{
		return new DirectedFlow(Flow * modifer, TargetIndex3D, OriginIndex3D);
	}
}
