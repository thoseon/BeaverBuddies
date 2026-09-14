using System.Collections.Generic;
using Timberborn.TickSystem;

namespace Timberborn.ModularShafts;

internal class ModularShaftAnimatorUpdater : ITickableSingleton
{
	private readonly HashSet<ModularShaftAnimator> _modularShaftAnimators = new HashSet<ModularShaftAnimator>();

	public void Tick()
	{
		foreach (ModularShaftAnimator modularShaftAnimator in _modularShaftAnimators)
		{
			modularShaftAnimator.UpdateAnimation();
		}
	}

	public void Register(ModularShaftAnimator modularShaftAnimator)
	{
		_modularShaftAnimators.Add(modularShaftAnimator);
	}

	public void Unregister(ModularShaftAnimator modularShaftAnimator)
	{
		_modularShaftAnimators.Remove(modularShaftAnimator);
	}
}
