using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.SliderToggleSystem;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Timberborn.WorkshopsUI;

public class ManufactoryTogglableRecipesBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ManufactoryRecipeSliderToggleFactory _manufactoryRecipeSliderToggleFactory;

	public ManufactoryTogglableRecipesBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ManufactoryRecipeSliderToggleFactory manufactoryRecipeSliderToggleFactory)
	{
		_visualElementLoader = visualElementLoader;
		_manufactoryRecipeSliderToggleFactory = manufactoryRecipeSliderToggleFactory;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		if ((bool)entity.GetComponent<ManufactoryTogglableRecipes>())
		{
			string elementName = "Game/BatchControl/SelectionToggleBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Manufactory component = entity.GetComponent<Manufactory>();
			SliderToggle sliderToggle = _manufactoryRecipeSliderToggleFactory.Create(visualElement, component);
			return new ManufactoryTogglableRecipesBatchControlRowItem(visualElement, sliderToggle);
		}
		return null;
	}
}
