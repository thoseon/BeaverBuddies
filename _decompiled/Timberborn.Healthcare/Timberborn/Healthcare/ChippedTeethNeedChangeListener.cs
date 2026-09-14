using Timberborn.BaseComponentSystem;
using Timberborn.NeedSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Healthcare;

internal class ChippedTeethNeedChangeListener : BaseComponent, IAwakableComponent
{
	private static readonly string ChippedTeethNeedId = "ChippedTeeth";

	private readonly EventBus _eventBus;

	public ChippedTeethNeedChangeListener(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Awake()
	{
		GetComponent<NeedManager>().NeedChangedActiveState += OnNeedChangedActiveState;
	}

	private void OnNeedChangedActiveState(object sender, NeedChangedActiveStateEventArgs e)
	{
		if (e.IsActive && e.NeedSpec.Id == ChippedTeethNeedId)
		{
			_eventBus.Post(new TeethChippedEvent());
		}
	}
}
