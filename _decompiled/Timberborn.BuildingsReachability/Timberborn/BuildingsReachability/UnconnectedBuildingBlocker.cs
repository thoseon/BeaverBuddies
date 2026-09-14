using System;

namespace Timberborn.BuildingsReachability;

public class UnconnectedBuildingBlocker : IUnconnectedBuildingBlocker
{
	public bool IsUnconnectedBlocked => true;

	public event EventHandler IsUnconnectedBlockedChanged;
}
