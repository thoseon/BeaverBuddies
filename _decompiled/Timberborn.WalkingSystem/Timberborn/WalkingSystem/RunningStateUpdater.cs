using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BeaverContaminationSystem;
using Timberborn.BonusSystem;
using Timberborn.CharacterMovementSystem;

namespace Timberborn.WalkingSystem;

internal class RunningStateUpdater : BaseComponent, IAwakableComponent
{
	private static readonly string MovementSpeedBonusName = "MovementSpeed";

	private BonusManager _bonusManager;

	private RunningProhibitor _runningProhibitor;

	private WalkingEnforcer _walkingEnforcer;

	private Contaminable _contaminable;

	private RunningStateUpdaterSpec _runningStateUpdaterSpec;

	private bool _isPathShort;

	private bool _isWalkingSpeedTooLow;

	public void Awake()
	{
		_bonusManager = GetComponent<BonusManager>();
		_bonusManager.BonusValueChanged += OnBonusValueChanged;
		_runningProhibitor = GetComponent<RunningProhibitor>();
		_walkingEnforcer = GetComponent<WalkingEnforcer>();
		WalkingEnforcer walkingEnforcer = _walkingEnforcer;
		walkingEnforcer.ForcedWalkingChanged = (EventHandler)Delegate.Combine(walkingEnforcer.ForcedWalkingChanged, new EventHandler(OnForcedWalkingChanged));
		_contaminable = GetComponent<Contaminable>();
		_runningStateUpdaterSpec = GetComponent<RunningStateUpdaterSpec>();
		if ((bool)_contaminable)
		{
			_contaminable.ContaminationChanged += OnContaminationChanged;
		}
		GetComponent<Walker>().StartedNewPath += OnStartedNewPath;
	}

	private void OnStartedNewPath(object sender, StartedNewPathEventArgs e)
	{
		_isPathShort = e.Distance < _runningStateUpdaterSpec.ShortWalkingDistanceThreshold;
		UpdateRunningProhibition();
	}

	private void OnBonusValueChanged(object sender, BonusValueChangedEventArgs e)
	{
		if (e.BonusId == MovementSpeedBonusName)
		{
			_isWalkingSpeedTooLow = e.Value < _runningStateUpdaterSpec.WalkingSpeedThreshold;
			UpdateRunningProhibition();
		}
	}

	private void OnForcedWalkingChanged(object sender, EventArgs e)
	{
		UpdateRunningProhibition();
	}

	private void OnContaminationChanged(object sender, EventArgs e)
	{
		UpdateRunningProhibition();
	}

	private void UpdateRunningProhibition()
	{
		_runningProhibitor.RunningProhibited = _isPathShort || _isWalkingSpeedTooLow || _walkingEnforcer.ForcedWalking || ((bool)_contaminable && _contaminable.IsContaminated);
	}
}
