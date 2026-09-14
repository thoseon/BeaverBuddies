using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.TimbermeshAnimations;
using Timberborn.TimeSystem;
using Timberborn.Workshops;

namespace Timberborn.WorkshopsEffects;

internal class WorkshopAnimationController : BaseComponent, IAwakableComponent, IPostLoadableEntity, IFinishedStateListener
{
	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private readonly EventBus _eventBus;

	private Workshop _workshop;

	private IAnimator _animator;

	private IWorkshopAnimationSpeedModifier _workshopAnimationSpeedModifier;

	public WorkshopAnimationController(NonlinearAnimationManager nonlinearAnimationManager, EventBus eventBus)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_workshop = GetComponent<Workshop>();
		_animator = GetComponentInChildren<IAnimator>(includeInactive: true);
		_workshopAnimationSpeedModifier = GetComponent<IWorkshopAnimationSpeedModifier>();
		if (_workshopAnimationSpeedModifier != null)
		{
			_workshopAnimationSpeedModifier.SpeedModifierChanged += OnSpeedModifierChanged;
		}
	}

	public void PostLoadEntity()
	{
		UpdateAnimatorState();
	}

	[OnEvent]
	public void OnCurrentSpeedChanged(CurrentSpeedChangedEvent currentSpeedChangedEvent)
	{
		UpdateAnimatorState();
	}

	public void OnEnterFinishedState()
	{
		_eventBus.Register(this);
		_workshop.WorkshopStateChanged += OnWorkshopStateChanged;
		UpdateAnimatorState();
	}

	public void OnExitFinishedState()
	{
		_eventBus.Unregister(this);
		_workshop.WorkshopStateChanged -= OnWorkshopStateChanged;
	}

	private void OnSpeedModifierChanged(object sender, EventArgs e)
	{
		UpdateAnimatorState();
	}

	private void OnWorkshopStateChanged(object sender, WorkshopStateChangedEventArgs e)
	{
		UpdateAnimatorState();
	}

	private void UpdateAnimatorState()
	{
		_animator.Enabled = _workshop.CurrentlyWorking;
		_animator.Speed = _nonlinearAnimationManager.SpeedMultiplier * (_workshopAnimationSpeedModifier?.SpeedModifier ?? 1f);
	}
}
