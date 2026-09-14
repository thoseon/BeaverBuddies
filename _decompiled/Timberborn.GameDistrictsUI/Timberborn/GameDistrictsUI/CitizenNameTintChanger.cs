using Timberborn.EntityNaming;
using Timberborn.SingletonSystem;

namespace Timberborn.GameDistrictsUI;

internal class CitizenNameTintChanger : ILoadableSingleton
{
	private readonly EventBus _eventBus;

	public CitizenNameTintChanger(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnEntityNameChanged(EntityNameChangedEvent entityNameChangedEvent)
	{
		CitizenTint component = entityNameChangedEvent.Entity.GetComponent<CitizenTint>();
		if ((bool)component)
		{
			component.UpdateTint();
		}
	}
}
