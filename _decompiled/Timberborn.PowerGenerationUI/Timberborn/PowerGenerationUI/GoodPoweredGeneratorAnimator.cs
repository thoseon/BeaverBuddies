using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.MechanicalSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.TimbermeshAnimations;
using Timberborn.TimeSystem;

namespace Timberborn.PowerGenerationUI;

internal class GoodPoweredGeneratorAnimator : TickableComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private readonly EventBus _eventBus;

	private MechanicalNode _mechanicalNode;

	private IAnimator _animator;

	public GoodPoweredGeneratorAnimator(NonlinearAnimationManager nonlinearAnimationManager, EventBus eventBus)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_animator = GetComponentInChildren<IAnimator>(includeInactive: true);
		DisableComponent();
	}

	public override void Tick()
	{
		UpdateAnimation();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		_eventBus.Register(this);
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		_eventBus.Unregister(this);
	}

	[OnEvent]
	public void OnCurrentSpeedChanged(CurrentSpeedChangedEvent currentSpeedChangedEvent)
	{
		UpdateAnimation();
	}

	private void UpdateAnimation()
	{
		if (_mechanicalNode.OutputMultiplier > 0f)
		{
			_animator.Enabled = true;
			_animator.Speed = _nonlinearAnimationManager.SpeedMultiplier;
		}
		else
		{
			_animator.Enabled = false;
		}
	}
}
