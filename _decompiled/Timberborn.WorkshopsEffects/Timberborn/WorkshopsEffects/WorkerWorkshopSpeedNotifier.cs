using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CharacterModelSystem;
using Timberborn.EnterableSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace Timberborn.WorkshopsEffects;

internal class WorkerWorkshopSpeedNotifier : BaseComponent, IAwakableComponent
{
	private static readonly string WorkshopSpeedKey = "WorkshopSpeed";

	private readonly EventBus _eventBus;

	private readonly NonlinearAnimationManager _nonlinearAnimationManager;

	private IWorkshopAnimationSpeedModifier _workshopAnimationSpeedModifier;

	private readonly List<CharacterAnimator> _workerCharacterAnimators = new List<CharacterAnimator>();

	public WorkerWorkshopSpeedNotifier(EventBus eventBus, NonlinearAnimationManager nonlinearAnimationManager)
	{
		_eventBus = eventBus;
		_nonlinearAnimationManager = nonlinearAnimationManager;
	}

	public void Awake()
	{
		_workshopAnimationSpeedModifier = GetComponent<IWorkshopAnimationSpeedModifier>();
		_workshopAnimationSpeedModifier.SpeedModifierChanged += delegate
		{
			UpdateWorkersSpeed();
		};
		Enterable component = GetComponent<Enterable>();
		component.EntererAdded += OnEntererAdded;
		component.EntererRemoved += OnEntererRemoved;
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnCurrentSpeedChanged(CurrentSpeedChangedEvent currentSpeedChangedEvent)
	{
		UpdateWorkersSpeed();
	}

	private void OnEntererAdded(object sender, EntererAddedEventArgs e)
	{
		CharacterAnimator component = e.Enterer.GetComponent<CharacterAnimator>();
		_workerCharacterAnimators.Add(component);
	}

	private void OnEntererRemoved(object sender, EntererRemovedEventArgs e)
	{
		CharacterAnimator component = e.Enterer.GetComponent<CharacterAnimator>();
		_workerCharacterAnimators.Remove(component);
	}

	private void UpdateWorkersSpeed()
	{
		float value = _workshopAnimationSpeedModifier.SpeedModifier * _nonlinearAnimationManager.SpeedMultiplier;
		foreach (CharacterAnimator workerCharacterAnimator in _workerCharacterAnimators)
		{
			workerCharacterAnimator.SetFloat(WorkshopSpeedKey, value);
		}
	}
}
