using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;

namespace Timberborn.WalkingSystem;

public class WalkingEnforcer : BaseComponent
{
	public EventHandler ForcedWalkingChanged;

	private readonly List<WalkingEnforcerToggle> _toggles = new List<WalkingEnforcerToggle>();

	public bool ForcedWalking { get; private set; }

	public WalkingEnforcerToggle GetWalkingEnforcerToggle()
	{
		WalkingEnforcerToggle walkingEnforcerToggle = new WalkingEnforcerToggle();
		_toggles.Add(walkingEnforcerToggle);
		walkingEnforcerToggle.ForcedWalkingChanged += OnForcedWalkingChanged;
		return walkingEnforcerToggle;
	}

	private void OnForcedWalkingChanged(object sender, EventArgs e)
	{
		bool flag = _toggles.FastAny((WalkingEnforcerToggle toggle) => toggle.ForcedWalking);
		if (ForcedWalking != flag)
		{
			ForcedWalking = flag;
			ForcedWalkingChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
