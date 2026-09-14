using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.SelectionSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.BeaversUI;

public class BeaverBuildingView
{
	private static readonly string HideDefaultClass = "beaver-buildings-fragment__icon--empty";

	private readonly EntitySelectionService _entitySelectionService;

	private readonly Image _buildingImage;

	private readonly Label _description;

	private Building _building;

	public Button Root { get; }

	public BeaverBuildingView(EntitySelectionService entitySelectionService, Button root, Image buildingImage, Label description)
	{
		_entitySelectionService = entitySelectionService;
		Root = root;
		Root.RegisterCallback<ClickEvent>(OnClick);
		_buildingImage = buildingImage;
		_description = description;
	}

	public void SetBuilding(Building building, string description)
	{
		Root.SetEnabled(value: true);
		_building = building;
		Sprite image = _building.GetComponent<LabeledEntity>().Image;
		_buildingImage.sprite = image;
		_buildingImage.AddToClassList(HideDefaultClass);
		_description.text = description;
	}

	public void SetDescriptionOnly(string description)
	{
		_buildingImage.sprite = null;
		_buildingImage.RemoveFromClassList(HideDefaultClass);
		Root.SetEnabled(value: false);
		_description.text = description;
	}

	private void OnClick(ClickEvent evt)
	{
		_entitySelectionService.SelectAndFocusOn(_building);
	}
}
