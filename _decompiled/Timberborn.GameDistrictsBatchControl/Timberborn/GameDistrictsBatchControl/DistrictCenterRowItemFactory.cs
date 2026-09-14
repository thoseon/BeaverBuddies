using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.SelectionSystem;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsBatchControl;

public class DistrictCenterRowItemFactory
{
	private readonly EntitySelectionService _entitySelectionService;

	private readonly VisualElementLoader _visualElementLoader;

	public DistrictCenterRowItemFactory(EntitySelectionService entitySelectionService, VisualElementLoader visualElementLoader)
	{
		_entitySelectionService = entitySelectionService;
		_visualElementLoader = visualElementLoader;
	}

	public IBatchControlRowItem Create(DistrictCenter districtCenter)
	{
		string elementName = "Game/BatchControl/DistrictCenterRowItem";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		LabeledEntity component = districtCenter.GetComponent<LabeledEntity>();
		visualElement.Q<Image>("Image").sprite = component.Image;
		visualElement.Q<Button>("Select").RegisterCallback<ClickEvent>(delegate
		{
			_entitySelectionService.SelectAndFocusOn(districtCenter);
		});
		return new DistrictCenterRowItem(visualElement, districtCenter, visualElement.Q<Label>("Text"));
	}
}
