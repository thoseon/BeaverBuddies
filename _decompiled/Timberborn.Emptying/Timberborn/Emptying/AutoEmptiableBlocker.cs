using System;
using Timberborn.BaseComponentSystem;

namespace Timberborn.Emptying;

public class AutoEmptiableBlocker : BaseComponent
{
	private int _blockingToggles;

	public bool IsBlocked { get; private set; }

	public event EventHandler BlockingStatusChanged;

	public AutoEmptiableBlockerToggle CreateToggle()
	{
		return new AutoEmptiableBlockerToggle(this);
	}

	internal void IncrementBlockingToggles()
	{
		if (_blockingToggles++ == 0)
		{
			UpdateBlocking(isBlocked: true);
		}
	}

	internal void DecrementBlockingToggles()
	{
		if (--_blockingToggles == 0)
		{
			UpdateBlocking(isBlocked: false);
		}
	}

	private void UpdateBlocking(bool isBlocked)
	{
		IsBlocked = isBlocked;
		BlockingStatusChanged?.Invoke(this, EventArgs.Empty);
	}
}
