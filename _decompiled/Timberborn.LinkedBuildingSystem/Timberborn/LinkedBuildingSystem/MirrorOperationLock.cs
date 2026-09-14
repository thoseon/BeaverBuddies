using System;

namespace Timberborn.LinkedBuildingSystem;

public class MirrorOperationLock
{
	private readonly struct MirrorLockToken(MirrorOperationLock mirrorOperationLock) : IDisposable
	{
		private readonly MirrorOperationLock _mirrorOperationLock = mirrorOperationLock;

		public void Dispose()
		{
			_mirrorOperationLock.Unlock();
		}
	}

	public bool IsUnlocked { get; private set; } = true;

	public IDisposable Lock()
	{
		if (!IsUnlocked)
		{
			throw new InvalidOperationException("Cannot lock an already locked MirrorOperationLock");
		}
		IsUnlocked = false;
		return new MirrorLockToken(this);
	}

	private void Unlock()
	{
		IsUnlocked = true;
	}
}
