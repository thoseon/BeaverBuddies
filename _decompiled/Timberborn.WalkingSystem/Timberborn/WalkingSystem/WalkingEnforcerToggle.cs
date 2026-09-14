using System;

namespace Timberborn.WalkingSystem;

public class WalkingEnforcerToggle
{
	public bool ForcedWalking { get; private set; }

	public event EventHandler ForcedWalkingChanged;

	public void EnableForcedWalking()
	{
		ForcedWalking = true;
		InvokeForcedWalkingChanged();
	}

	public void DisableForcedWalking()
	{
		ForcedWalking = false;
		InvokeForcedWalkingChanged();
	}

	private void InvokeForcedWalkingChanged()
	{
		ForcedWalkingChanged?.Invoke(this, EventArgs.Empty);
	}
}
