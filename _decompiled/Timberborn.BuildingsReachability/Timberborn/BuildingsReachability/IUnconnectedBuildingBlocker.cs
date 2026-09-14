using System;

namespace Timberborn.BuildingsReachability;

public interface IUnconnectedBuildingBlocker
{
	bool IsUnconnectedBlocked { get; }

	event EventHandler IsUnconnectedBlockedChanged;
}
