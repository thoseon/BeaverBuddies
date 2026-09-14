using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimbermeshAnimations;
using Timberborn.TimeSystem;

namespace Timberborn.BlockingSystem;

internal class BlockableObjectAnimationController : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private readonly EventBus _eventBus;

	private IAnimator _animator;

	private BlockableObject _blockableObject;

	public BlockableObjectAnimationController(NonlinearAnimationManager nonlinearAnimationManager, EventBus eventBus)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
		_animator = GetComponentInChildren<IAnimator>(includeInactive: true);
	}

	[OnEvent]
	public void OnCurrentSpeedChanged(CurrentSpeedChangedEvent currentSpeedChangedEvent)
	{
		UpdateAnimatorState();
	}

	public void OnEnterFinishedState()
	{
		_eventBus.Register(this);
		_blockableObject.ObjectUnblocked += OnObjectUnblocked;
		_blockableObject.ObjectBlocked += OnObjectBlocked;
		UpdateAnimatorState();
	}

	public void OnExitFinishedState()
	{
		_eventBus.Unregister(this);
		_blockableObject.ObjectUnblocked -= OnObjectUnblocked;
		_blockableObject.ObjectBlocked -= OnObjectBlocked;
	}

	private void OnObjectUnblocked(object sender, EventArgs e)
	{
		UpdateAnimatorState();
	}

	private void OnObjectBlocked(object sender, EventArgs e)
	{
		UpdateAnimatorState();
	}

	private void UpdateAnimatorState()
	{
		_animator.Enabled = _blockableObject.IsUnblocked;
		_animator.Speed = _nonlinearAnimationManager.SpeedMultiplier;
	}
}
