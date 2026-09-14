using UnityEngine;

namespace Timberborn.StatusSystem;

internal readonly struct SlotConstraints
{
	public bool IsOccupied { get; }

	public byte BaseZ { get; }

	public bool InvalidInConstructionMode { get; }

	public bool ForceValidInConstructionMode { get; }

	private SlotConstraints(bool isOccupied, byte baseZ, bool invalidInConstructionMode, bool forceValidInConstructionMode)
	{
		IsOccupied = isOccupied;
		BaseZ = baseZ;
		InvalidInConstructionMode = invalidInConstructionMode;
		ForceValidInConstructionMode = forceValidInConstructionMode;
	}

	public static SlotConstraints GetOccupied(byte baseZ, bool invalidInConstructionMode = false, bool forceValidInConstructionMode = false)
	{
		return new SlotConstraints(isOccupied: true, baseZ, invalidInConstructionMode, forceValidInConstructionMode);
	}

	public static SlotConstraints GetUnoccupied(byte baseZ)
	{
		return new SlotConstraints(isOccupied: false, baseZ, invalidInConstructionMode: false, forceValidInConstructionMode: false);
	}

	public SlotConstraints Merge(SlotConstraints other)
	{
		bool isOccupied = IsOccupied || other.IsOccupied;
		byte baseZ = (byte)Mathf.Min(BaseZ, other.BaseZ);
		bool invalidInConstructionMode = BothInvalidInConstructionModeOrEmpty(other);
		bool forceValidInConstructionMode = ForceValidInConstructionMode || other.ForceValidInConstructionMode;
		return new SlotConstraints(isOccupied, baseZ, invalidInConstructionMode, forceValidInConstructionMode);
	}

	private bool BothInvalidInConstructionModeOrEmpty(SlotConstraints other)
	{
		if ((InvalidInConstructionMode || other.InvalidInConstructionMode) && (!IsOccupied || InvalidInConstructionMode))
		{
			if (other.IsOccupied)
			{
				return other.InvalidInConstructionMode;
			}
			return true;
		}
		return false;
	}
}
