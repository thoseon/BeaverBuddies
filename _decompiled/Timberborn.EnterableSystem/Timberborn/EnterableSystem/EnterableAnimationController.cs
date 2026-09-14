using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimbermeshAnimations;
using Timberborn.TimeSystem;

namespace Timberborn.EnterableSystem;

public class EnterableAnimationController : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private readonly EventBus _eventBus;

	private Enterable _enterable;

	private IAnimator _animator;

	private EnterableAnimationControllerSpec _enterableAnimationControllerSpec;

	public EnterableAnimationController(NonlinearAnimationManager nonlinearAnimationManager, EventBus eventBus)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_enterable = GetComponent<Enterable>();
		_animator = GetComponentInChildren<IAnimator>(includeInactive: true);
		_enterableAnimationControllerSpec = GetComponent<EnterableAnimationControllerSpec>();
		DisableComponent();
	}

	public void OnEnterFinishedState()
	{
		_eventBus.Register(this);
		_enterable.EntererAdded += OnEntererAdded;
		_enterable.EntererRemoved += OnEntererRemoved;
		EnableComponent();
	}

	public void OnExitFinishedState()
	{
		_eventBus.Unregister(this);
		_enterable.EntererAdded -= OnEntererAdded;
		_enterable.EntererRemoved -= OnEntererRemoved;
		DisableComponent();
	}

	[OnEvent]
	public void OnCurrentSpeedChanged(CurrentSpeedChangedEvent currentSpeedChangedEvent)
	{
		UpdateAnimatorState();
	}

	private void OnEntererRemoved(object sender, EntererRemovedEventArgs e)
	{
		if (_enterableAnimationControllerSpec.ResetAnimationUponExit)
		{
			_animator.SetTime(0f);
		}
		UpdateAnimatorState();
	}

	private void OnEntererAdded(object sender, EntererAddedEventArgs e)
	{
		UpdateAnimatorState();
	}

	private void UpdateAnimatorState()
	{
		_animator.Enabled = _enterable.NumberOfEnterersInside > 0;
		_animator.Speed = _nonlinearAnimationManager.SpeedMultiplier;
	}
}
