using Timberborn.BatchControl;
using Timberborn.Characters;
using UnityEngine.UIElements;

namespace Timberborn.CharactersUI;

internal class CharacterBatchControlRowItem : IBatchControlRowItem, IUpdatableBatchControlRowItem
{
	private readonly Label _entityName;

	private readonly Character _character;

	public VisualElement Root { get; }

	public CharacterBatchControlRowItem(VisualElement root, Label entityName, Character character)
	{
		Root = root;
		_entityName = entityName;
		_character = character;
	}

	public void UpdateRowItem()
	{
		_entityName.text = _character.FirstName;
	}
}
