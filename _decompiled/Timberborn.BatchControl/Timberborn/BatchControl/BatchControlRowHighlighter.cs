using Timberborn.EntitySystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.BatchControl;

internal class BatchControlRowHighlighter : ILoadableSingleton
{
	private static readonly string HighlightedClass = "batch-control-box__row--highlighted";

	private readonly EventBus _eventBus;

	private readonly BatchControlBoxTabController _batchControlBoxTabController;

	private readonly EntitySelectionService _entitySelectionService;

	public BatchControlRowHighlighter(EventBus eventBus, BatchControlBoxTabController batchControlBoxTabController, EntitySelectionService entitySelectionService)
	{
		_eventBus = eventBus;
		_batchControlBoxTabController = batchControlBoxTabController;
		_entitySelectionService = entitySelectionService;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnBatchControlTabShown(BatchControlTabShownEvent batchControlTabShownEvent)
	{
		if (_entitySelectionService.IsAnythingSelected)
		{
			SetEntityRowsHighlight(batchControlTabShownEvent.BatchControlTab, _entitySelectionService.SelectedObject.GetComponent<EntityComponent>(), isHighlighted: true);
		}
	}

	[OnEvent]
	public void OnSelectableObjectSelected(SelectableObjectSelectedEvent selectableObjectSelectedEvent)
	{
		EntityComponent component = selectableObjectSelectedEvent.SelectableObject.GetComponent<EntityComponent>();
		foreach (BatchControlTab tab in _batchControlBoxTabController.Tabs)
		{
			SetEntityRowsHighlight(tab, component, isHighlighted: true);
		}
	}

	[OnEvent]
	public void OnSelectableObjectUnselected(SelectableObjectUnselectedEvent selectableObjectUnselectedEvent)
	{
		SelectableObject selectableObject = selectableObjectUnselectedEvent.SelectableObject;
		if (!selectableObject)
		{
			return;
		}
		foreach (BatchControlTab tab in _batchControlBoxTabController.Tabs)
		{
			SetEntityRowsHighlight(tab, selectableObject.GetComponent<EntityComponent>(), isHighlighted: false);
		}
	}

	private static void SetEntityRowsHighlight(BatchControlTab batchControlTab, EntityComponent entity, bool isHighlighted)
	{
		foreach (BatchControlRow entityRow in batchControlTab.GetEntityRows(entity))
		{
			entityRow.Root.EnableInClassList(HighlightedClass, isHighlighted);
		}
	}
}
