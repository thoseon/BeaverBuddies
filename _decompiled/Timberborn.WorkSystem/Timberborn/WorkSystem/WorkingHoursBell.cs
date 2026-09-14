using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;
using Timberborn.SoundSystem;
using Timberborn.TimbermeshAnimations;

namespace Timberborn.WorkSystem;

public class WorkingHoursBell : BaseComponent, IAwakableComponent, IInitializableEntity, IFinishedStateListener
{
	private readonly ISoundSystem _soundSystem;

	private readonly EventBus _eventBus;

	private IAnimator _animator;

	private BuildingSoundController _buildingSoundController;

	public WorkingHoursBell(ISoundSystem soundSystem, EventBus eventBus)
	{
		_soundSystem = soundSystem;
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_animator = GetComponentInChildren<IAnimator>(includeInactive: true);
		_buildingSoundController = GetComponent<BuildingSoundController>();
	}

	public void InitializeEntity()
	{
		_animator.Enabled = true;
		_animator.Stop();
	}

	public void OnEnterFinishedState()
	{
		_eventBus.Register(this);
	}

	public void OnExitFinishedState()
	{
		_eventBus.Unregister(this);
	}

	[OnEvent]
	public void OnWorkingHoursTransitioned(WorkingHoursTransitionedEvent workingHoursTransitionedEvent)
	{
		PlayBellSound();
		AnimateBellToll();
	}

	private void PlayBellSound()
	{
		if (_buildingSoundController.PlaySound)
		{
			_soundSystem.PlaySound3D(base.GameObject, "Environment.Buildings.BellToll", 30);
		}
	}

	private void AnimateBellToll()
	{
		_animator.Play("Default", looped: false);
	}
}
