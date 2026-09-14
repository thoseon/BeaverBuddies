using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.MechanicalSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.TimbermeshAnimations;
using Timberborn.TimeSystem;

namespace Timberborn.MechanicalSystemUI;

internal class MechanicalNodeAnimator : TickableComponent, IAwakableComponent, IPostLoadableEntity, IFinishedStateListener, IUnfinishedStateListener
{
	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private readonly EventBus _eventBus;

	private MechanicalNode _mechanicalNode;

	private MechanicalNodeAnimatorSpec _mechanicalNodeAnimatorSpec;

	private IAnimator[] _animators;

	private IAnimator _activeAnimator;

	private float _currentAnimationSpeed;

	public MechanicalNodeAnimator(NonlinearAnimationManager nonlinearAnimationManager, EventBus eventBus)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_mechanicalNodeAnimatorSpec = GetComponent<MechanicalNodeAnimatorSpec>();
		_animators = base.GameObject.GetComponentsInChildren<IAnimator>(includeInactive: true);
		StopAnimators();
		DisableComponent();
	}

	public void PostLoadEntity()
	{
		FindAndUpdateActiveAnimator();
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

	public void OnEnterUnfinishedState()
	{
		StopAnimators();
	}

	public void OnExitUnfinishedState()
	{
	}

	[OnEvent]
	public void OnCurrentSpeedChanged(CurrentSpeedChangedEvent currentSpeedChangedEvent)
	{
		UpdateAnimation();
	}

	private void StopAnimators()
	{
		IAnimator[] animators = _animators;
		for (int i = 0; i < animators.Length; i++)
		{
			animators[i].Enabled = false;
		}
	}

	private void FindAndUpdateActiveAnimator()
	{
		_activeAnimator = GetComponentInChildren<IAnimator>();
		StopAnimators();
		UpdateAnimation();
	}

	private void UpdateAnimation()
	{
		if (_activeAnimator != null)
		{
			if (CanAnimate())
			{
				_activeAnimator.Enabled = true;
				_activeAnimator.Speed = GetPowerMultiplier() * _nonlinearAnimationManager.SpeedMultiplier;
			}
			else
			{
				_activeAnimator.Enabled = false;
			}
		}
	}

	private bool CanAnimate()
	{
		if (_mechanicalNode.ActiveAndPowered)
		{
			if (!_mechanicalNode.IsConsuming && !_mechanicalNode.IsGenerator && (!_mechanicalNode.IsShaft || !_mechanicalNode.Powered))
			{
				if (_mechanicalNode.IsIntermediary)
				{
					return _mechanicalNode.Powered;
				}
				return false;
			}
			return true;
		}
		return false;
	}

	private float GetPowerMultiplier()
	{
		float powerEfficiency = _mechanicalNode.PowerEfficiency;
		float minSpeedMultiplier = _mechanicalNodeAnimatorSpec.MinSpeedMultiplier;
		return (powerEfficiency + minSpeedMultiplier) / (1f + minSpeedMultiplier);
	}
}
