using Timberborn.SelectionSystem;
using UnityEngine.UIElements;

namespace Timberborn.BeaversUI;

public class BeaverBuildingViewFactory
{
	private readonly EntitySelectionService _entitySelectionService;

	public BeaverBuildingViewFactory(EntitySelectionService entitySelectionService)
	{
		_entitySelectionService = entitySelectionService;
	}

	public BeaverBuildingView Create(Button root)
	{
		Image buildingImage = root.Q<Image>("Icon");
		Label description = root.Q<Label>("Name");
		return new BeaverBuildingView(_entitySelectionService, root, buildingImage, description);
	}
}
