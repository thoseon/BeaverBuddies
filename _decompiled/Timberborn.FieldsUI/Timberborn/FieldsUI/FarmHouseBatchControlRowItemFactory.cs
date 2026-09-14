using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.Fields;
using UnityEngine.UIElements;

namespace Timberborn.FieldsUI;

public class FarmHouseBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly FarmHouseToggleFactory _farmHouseToggleFactory;

	public FarmHouseBatchControlRowItemFactory(VisualElementLoader visualElementLoader, FarmHouseToggleFactory farmHouseToggleFactory)
	{
		_visualElementLoader = visualElementLoader;
		_farmHouseToggleFactory = farmHouseToggleFactory;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		FarmHouse component = entity.GetComponent<FarmHouse>();
		if (component != null)
		{
			string elementName = "Game/BatchControl/SelectionToggleBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			FarmHouseToggle farmHouseToggle = _farmHouseToggleFactory.Create(visualElement);
			farmHouseToggle.Show(component);
			return new FarmHouseBatchControlRowItem(visualElement, farmHouseToggle);
		}
		return null;
	}
}
