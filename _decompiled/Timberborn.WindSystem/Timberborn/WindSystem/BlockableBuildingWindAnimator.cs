using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockingSystem;

namespace Timberborn.WindSystem;

internal class BlockableBuildingWindAnimator : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private BlockableObject _blockableObject;

	private WindRotationAnimator _windRotationAnimator;

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_windRotationAnimator = GetComponent<WindRotationAnimator>();
	}

	public void OnEnterFinishedState()
	{
		_blockableObject.ObjectBlocked += OnBlocked;
		_blockableObject.ObjectUnblocked += OnUnblocked;
		if (_blockableObject.IsUnblocked)
		{
			_windRotationAnimator.UnsuspendAnimation();
		}
		else
		{
			_windRotationAnimator.SuspendAnimation();
		}
	}

	public void OnExitFinishedState()
	{
		_blockableObject.ObjectBlocked -= OnBlocked;
		_blockableObject.ObjectUnblocked -= OnUnblocked;
	}

	private void OnUnblocked(object sender, EventArgs e)
	{
		_windRotationAnimator.UnsuspendAnimation();
	}

	private void OnBlocked(object sender, EventArgs e)
	{
		_windRotationAnimator.SuspendAnimation();
	}
}
