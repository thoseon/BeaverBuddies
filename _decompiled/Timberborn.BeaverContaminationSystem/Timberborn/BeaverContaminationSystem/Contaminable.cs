using System;
using Timberborn.BaseComponentSystem;
using Timberborn.Effects;
using Timberborn.NeedSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.BeaverContaminationSystem;

public class Contaminable : BaseComponent, IAwakableComponent
{
	private static readonly string ContaminationNeedId = "BadwaterContamination";

	private readonly EventBus _eventBus;

	private NeedManager _needManager;

	public bool IsContaminated => _needManager.NeedIsActive(ContaminationNeedId);

	public event EventHandler ContaminationChanged;

	public Contaminable(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Awake()
	{
		_needManager = GetComponent<NeedManager>();
		_needManager.NeedChangedActiveState += OnNeedChangedActiveState;
	}

	public void Contaminate()
	{
		_needManager.ApplyEffect(new InstantEffect(ContaminationNeedId, float.MinValue, 1));
	}

	private void OnNeedChangedActiveState(object sender, NeedChangedActiveStateEventArgs e)
	{
		if (e.NeedSpec.Id == ContaminationNeedId)
		{
			ContaminationChanged?.Invoke(this, EventArgs.Empty);
			_eventBus.Post(new ContaminableContaminationChangedEvent(this));
		}
	}
}
