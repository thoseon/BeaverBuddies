using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.Bots;
using Timberborn.CoreUI;
using Timberborn.WorkSystem;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

public class WorkplaceWorkerTypeBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly WorkerTypeToggleFactory _workerTypeToggleFactory;

	private readonly BotPopulation _botPopulation;

	public WorkplaceWorkerTypeBatchControlRowItemFactory(VisualElementLoader visualElementLoader, WorkerTypeToggleFactory workerTypeToggleFactory, BotPopulation botPopulation)
	{
		_visualElementLoader = visualElementLoader;
		_workerTypeToggleFactory = workerTypeToggleFactory;
		_botPopulation = botPopulation;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		if (_botPopulation.BotCreated)
		{
			WorkplaceWorkerType component = entity.GetComponent<WorkplaceWorkerType>();
			if (component != null)
			{
				string elementName = "Game/BatchControl/SelectionToggleBatchControlRowItem";
				VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
				WorkerTypeToggle workerTypeToggle = _workerTypeToggleFactory.Create(visualElement);
				workerTypeToggle.Show(component);
				return new WorkplaceWorkerTypeBatchControlRowItem(visualElement, workerTypeToggle);
			}
		}
		return null;
	}
}
