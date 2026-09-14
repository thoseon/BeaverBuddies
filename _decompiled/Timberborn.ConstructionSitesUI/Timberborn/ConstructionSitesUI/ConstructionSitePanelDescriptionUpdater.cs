using Timberborn.BlockSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;

namespace Timberborn.ConstructionSitesUI;

public class ConstructionSitePanelDescriptionUpdater : ILoadableSingleton
{
	private readonly IEntityPanel _entityPanel;

	private readonly EventBus _eventBus;

	public ConstructionSitePanelDescriptionUpdater(IEntityPanel entityPanel, EventBus eventBus)
	{
		_entityPanel = entityPanel;
		_eventBus = eventBus;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnEnteredFinishedState(EnteredFinishedStateEvent enteredFinishedStateEvent)
	{
		BlockObject blockObject = enteredFinishedStateEvent.BlockObject;
		_entityPanel.ReloadDescription(blockObject.GetComponent<EntityComponent>());
	}
}
