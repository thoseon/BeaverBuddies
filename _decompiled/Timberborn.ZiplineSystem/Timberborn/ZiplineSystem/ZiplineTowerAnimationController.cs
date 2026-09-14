using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimbermeshAnimations;
using Timberborn.TimeSystem;

namespace Timberborn.ZiplineSystem;

internal class ZiplineTowerAnimationController : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private readonly EventBus _eventBus;

	private ZiplineTowerOperationValidator _ziplineTowerOperationValidator;

	private IAnimator _animator;

	public ZiplineTowerAnimationController(NonlinearAnimationManager nonlinearAnimationManager, EventBus eventBus)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_ziplineTowerOperationValidator = GetComponent<ZiplineTowerOperationValidator>();
		_animator = GetComponentInChildren<IAnimator>(includeInactive: true);
	}

	[OnEvent]
	public void OnCurrentSpeedChanged(CurrentSpeedChangedEvent currentSpeedChangedEvent)
	{
		UpdateAnimatorSpeed();
	}

	public void OnEnterFinishedState()
	{
		_eventBus.Register(this);
		_ziplineTowerOperationValidator.OperativeStateChanged += OnOperativeStateChanged;
		UpdateAnimatorState();
	}

	public void OnExitFinishedState()
	{
		_eventBus.Unregister(this);
		_ziplineTowerOperationValidator.OperativeStateChanged -= OnOperativeStateChanged;
	}

	private void OnOperativeStateChanged(object sender, EventArgs e)
	{
		UpdateAnimatorState();
	}

	private void UpdateAnimatorState()
	{
		_animator.Enabled = _ziplineTowerOperationValidator.IsOperative;
		UpdateAnimatorSpeed();
	}

	private void UpdateAnimatorSpeed()
	{
		_animator.Speed = _nonlinearAnimationManager.SpeedMultiplier;
	}
}
