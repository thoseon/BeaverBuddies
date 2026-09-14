using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.Characters;
using Timberborn.CoreUI;
using Timberborn.EntityNaming;
using Timberborn.EntityNamingUI;
using Timberborn.EntityPanelSystem;
using Timberborn.SelectionSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CharactersUI;

public class CharacterBatchControlRowItemFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly EntityBadgeService _entityBadgeService;

	private readonly EntityNameDialog _entityNameDialog;

	private readonly EntitySelectionService _entitySelectionService;

	public CharacterBatchControlRowItemFactory(VisualElementLoader visualElementLoader, EntityBadgeService entityBadgeService, EntityNameDialog entityNameDialog, EntitySelectionService entitySelectionService)
	{
		_visualElementLoader = visualElementLoader;
		_entityBadgeService = entityBadgeService;
		_entityNameDialog = entityNameDialog;
		_entitySelectionService = entitySelectionService;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		string elementName = "Game/BatchControl/CharacterBatchControlRowItem";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		Character character = entity.GetComponent<Character>();
		Button button = visualElement.Q<Button>("EntityAvatar");
		button.RegisterCallback<ClickEvent>(delegate
		{
			_entitySelectionService.SelectAndFollow(entity);
		});
		Sprite entityAvatar = _entityBadgeService.GetEntityAvatar(character);
		button.style.backgroundImage = new StyleBackground(entityAvatar);
		visualElement.Q<Button>("EntityName").RegisterCallback<ClickEvent>(delegate
		{
			OnEntityNameClicked(character);
		});
		return new CharacterBatchControlRowItem(visualElement, visualElement.Q<Label>("EntityNameText"), character);
	}

	private void OnEntityNameClicked(Character character)
	{
		_entityNameDialog.Show(character.GetComponent<NamedEntity>());
	}
}
