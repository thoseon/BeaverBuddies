using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.MechanicalSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.TimbermeshAnimations;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.PowerManagement;

internal class ClutchModel : TickableComponent, IAwakableComponent, IInitializablePreview, IFinishedStateListener, IUnfinishedStateListener
{
	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private readonly EventBus _eventBus;

	private Clutch _clutch;

	private MechanicalNode _mechanicalNode;

	private GameObject _engagedModel;

	private GameObject _disengagedModel;

	private IAnimator _engagedAnimator;

	private IAnimator _disengagedAnimator;

	public ClutchModel(NonlinearAnimationManager nonlinearAnimationManager, EventBus eventBus)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		ClutchModelSpec component = GetComponent<ClutchModelSpec>();
		_clutch = GetComponent<Clutch>();
		_mechanicalNode = GetComponent<MechanicalNode>();
		_engagedModel = base.GameObject.FindChild(component.EngagedModelName);
		_disengagedModel = base.GameObject.FindChild(component.DisengagedModelName);
		_engagedAnimator = _engagedModel.GetComponentInChildren<IAnimator>(includeInactive: true);
		_disengagedAnimator = _disengagedModel.GetComponentInChildren<IAnimator>(includeInactive: true);
		DisableComponent();
	}

	public void InitializePreview()
	{
		UpdateActiveModels();
	}

	public override void Tick()
	{
		UpdateModels();
	}

	public void OnEnterFinishedState()
	{
		UpdateModels();
		_clutch.IsEngagedChanged += OnIsEngagedChanged;
		_eventBus.Register(this);
		if (_clutch.IsEngaged)
		{
			EnableComponent();
		}
	}

	public void OnExitFinishedState()
	{
		_engagedAnimator.Enabled = false;
		_clutch.IsEngagedChanged -= OnIsEngagedChanged;
		_eventBus.Unregister(this);
		DisableComponent();
	}

	public void OnEnterUnfinishedState()
	{
		UpdateActiveModels();
		_clutch.IsEngagedChanged += OnIsEngagedChanged;
	}

	public void OnExitUnfinishedState()
	{
		_clutch.IsEngagedChanged -= OnIsEngagedChanged;
	}

	[OnEvent]
	public void OnCurrentSpeedChanged(CurrentSpeedChangedEvent currentSpeedChangedEvent)
	{
		UpdateAnimators();
	}

	private void OnIsEngagedChanged(object sender, EventArgs e)
	{
		UpdateModels();
		if (_clutch.IsEngaged)
		{
			_engagedAnimator.SetTime(_disengagedAnimator.Time);
			EnableComponent();
		}
		else
		{
			_disengagedAnimator.SetTime(_engagedAnimator.Time);
			DisableComponent();
		}
	}

	private void UpdateModels()
	{
		UpdateActiveModels();
		UpdateAnimators();
	}

	private void UpdateActiveModels()
	{
		_engagedModel.SetActive(_clutch.IsEngaged);
		_disengagedModel.SetActive(!_clutch.IsEngaged);
	}

	private void UpdateAnimators()
	{
		bool activeAndPowered = _mechanicalNode.ActiveAndPowered;
		_engagedAnimator.Enabled = activeAndPowered;
		if (activeAndPowered)
		{
			_engagedAnimator.Speed = _mechanicalNode.PowerEfficiency * _nonlinearAnimationManager.SpeedMultiplier;
		}
	}
}
