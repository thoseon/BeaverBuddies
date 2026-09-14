namespace Timberborn.StatusSystem;

public readonly struct StatusSlot
{
	public float ZCoordinate { get; }

	public bool InvalidInConstructionMode { get; }

	public byte BaseZ { get; }

	public byte UnfinishedBaseZ { get; }

	private StatusSlot(float zCoordinate, bool invalidInConstructionMode, byte baseZ, byte unfinishedBaseZ)
	{
		ZCoordinate = zCoordinate;
		InvalidInConstructionMode = invalidInConstructionMode;
		BaseZ = baseZ;
		UnfinishedBaseZ = unfinishedBaseZ;
	}

	public static StatusSlot CreateAlwaysValid(float statusZCoordinate)
	{
		return new StatusSlot(statusZCoordinate, invalidInConstructionMode: false, byte.MaxValue, byte.MaxValue);
	}

	public static StatusSlot CreateInvalidInConstructionMode(float statusZCoordinate, byte unfinishedBaseZ)
	{
		return new StatusSlot(statusZCoordinate, invalidInConstructionMode: true, byte.MaxValue, unfinishedBaseZ);
	}

	public static StatusSlot CreateValidAboveMaxVisibilityLevel(float statusZCoordinate, byte baseZ)
	{
		return new StatusSlot(statusZCoordinate, invalidInConstructionMode: false, baseZ, baseZ);
	}
}
