using System;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimbermeshAnimations;
using Timberborn.TimeSystem;

namespace Timberborn.AutomationBuildings;

internal class SpeakerAnimationController : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private readonly EventBus _eventBus;

	private IAnimator _animator;

	private Speaker _speaker;

	public SpeakerAnimationController(NonlinearAnimationManager nonlinearAnimationManager, EventBus eventBus)
	{
		_nonlinearAnimationManager = nonlinearAnimationManager;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_animator = GetComponentInChildren<IAnimator>(includeInactive: true);
		_speaker = GetComponent<Speaker>();
	}

	public void OnEnterFinishedState()
	{
		_eventBus.Register(this);
		_speaker.PlaybackStateChanged += OnPlaybackStateChanged;
	}

	public void OnExitFinishedState()
	{
		_eventBus.Unregister(this);
		_speaker.PlaybackStateChanged -= OnPlaybackStateChanged;
	}

	[OnEvent]
	public void OnCurrentSpeedChanged(CurrentSpeedChangedEvent currentSpeedChangedEvent)
	{
		UpdateAnimator();
	}

	private void OnPlaybackStateChanged(object sender, EventArgs e)
	{
		UpdateAnimator();
	}

	private void UpdateAnimator()
	{
		_animator.Enabled = _speaker.IsPlaying;
		_animator.Speed = _nonlinearAnimationManager.SpeedMultiplier;
	}
}
