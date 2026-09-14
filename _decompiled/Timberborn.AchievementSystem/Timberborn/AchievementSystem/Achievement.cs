using System;

namespace Timberborn.AchievementSystem;

public abstract class Achievement
{
	public abstract string Id { get; }

	public bool IsEnabled => UnlockCallback != null;

	private event EventHandler UnlockCallback;

	public void Enable(EventHandler unlockCallback)
	{
		UnlockCallback = unlockCallback;
		EnableInternal();
	}

	public void Unlock()
	{
		UnlockCallback?.Invoke(this, EventArgs.Empty);
		Disable();
	}

	protected void Disable()
	{
		if (IsEnabled)
		{
			UnlockCallback = null;
			DisableInternal();
		}
	}

	protected virtual void EnableInternal()
	{
	}

	protected virtual void DisableInternal()
	{
	}
}
