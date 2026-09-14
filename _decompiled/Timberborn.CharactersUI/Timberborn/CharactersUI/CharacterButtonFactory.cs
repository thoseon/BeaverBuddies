using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.GameFactionSystem;
using UnityEngine.UIElements;

namespace Timberborn.CharactersUI;

public class CharacterButtonFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly FactionService _factionService;

	private readonly EntityBadgeService _entityBadgeService;

	public CharacterButtonFactory(VisualElementLoader visualElementLoader, FactionService factionService, EntityBadgeService entityBadgeService)
	{
		_visualElementLoader = visualElementLoader;
		_factionService = factionService;
		_entityBadgeService = entityBadgeService;
	}

	public CharacterButton Create()
	{
		VisualElement root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/CharacterButton");
		return Create(root);
	}

	public CharacterButton Create(VisualElement root)
	{
		CharacterButton characterButton = new CharacterButton(root, _factionService, _entityBadgeService);
		characterButton.Initialize();
		return characterButton;
	}
}
