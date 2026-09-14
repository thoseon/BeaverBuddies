using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.GameDistrictsMigration;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class CurrentPopulationBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	public CurrentPopulationBatchControlRowItemFactory(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public IBatchControlRowItem Create(PopulationDistributor populationDistributor, string iconClass)
	{
		string elementName = "Game/BatchControl/PopulationBatchControlRowItem";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		visualElement.Q<VisualElement>("PopulationIcon").AddToClassList(iconClass);
		return new CurrentPopulationBatchControlRowItem(visualElement, visualElement.Q<Label>("CurrentPopulation"), populationDistributor);
	}
}
