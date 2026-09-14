using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;
using Timberborn.MechanicalSystem;
using Timberborn.TimbermeshAnimations;
using Timberborn.TimeSystem;

namespace Timberborn.ModularShafts;

internal class ModularShaftAnimator : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener, IUnfinishedStateListener
{
	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private readonly ModularShaftAnimatorUpdater _modularShaftAnimatorUpdater;

	private MechanicalNode _mechanicalNode;

	private ModularShaftModelUpdater _modularShaftModelUpdater;

	private readonly List<IAnimator> _animators = new List<IAnimator>();

	private float _currentAnimationSpeed;

	public bool IsAnimated { get; private set; }

	public ModularShaftAnimator(NonlinearAnimationManager nonlinearAnimationManager, ModularShaftAnimatorUpdater modularShaftAnimatorUpdater)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
		_modularShaftAnimatorUpdater = modularShaftAnimatorUpdater;
	}

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_modularShaftModelUpdater = GetComponent<ModularShaftModelUpdater>();
		_modularShaftModelUpdater.ModelUpdated += delegate
		{
			UpdateAll();
		};
		DisableComponent();
	}

	public void InitializeEntity()
	{
		UpdateAll();
	}

	public void OnEnterFinishedState()
	{
		EnableComponent();
		_modularShaftAnimatorUpdater.Register(this);
	}

	public void OnExitFinishedState()
	{
		DisableComponent();
		_modularShaftAnimatorUpdater.Unregister(this);
	}

	public void OnEnterUnfinishedState()
	{
		StopAnimators();
	}

	public void OnExitUnfinishedState()
	{
	}

	public void UpdateAnimation()
	{
		IsAnimated = _mechanicalNode.ActiveAndPowered && _mechanicalNode.PowerEfficiency > 0f;
		foreach (IAnimator animator in _animators)
		{
			animator.Enabled = IsAnimated;
			if (IsAnimated)
			{
				animator.Speed = _mechanicalNode.PowerEfficiency * _nonlinearAnimationManager.SpeedMultiplier;
			}
		}
	}

	private void StopAnimators()
	{
		foreach (IAnimator animator in _animators)
		{
			animator.Enabled = false;
		}
	}

	private void UpdateAll()
	{
		StopAnimators();
		CollectActiveAnimators();
		UpdateAnimation();
	}

	private void CollectActiveAnimators()
	{
		_animators.Clear();
		_animators.AddRange(base.GameObject.GetComponentsInChildren<IAnimator>(includeInactive: true));
	}
}
