using Timberborn.BaseComponentSystem;
using Timberborn.SelectionSystem;

namespace Timberborn.CharacterModelSystem;

internal class VisibleSelectedCharacterModel : BaseComponent, IAwakableComponent, ISelectionListener
{
	private CharacterModelBlockadeIgnoringToggle _characterModelBlockadeIgnoringToggle;

	public void Awake()
	{
		_characterModelBlockadeIgnoringToggle = GetComponent<CharacterModel>().CreateBlockadeIgnoringToggle();
	}

	public void OnSelect()
	{
		_characterModelBlockadeIgnoringToggle.Block();
	}

	public void OnUnselect()
	{
		_characterModelBlockadeIgnoringToggle.Unblock();
	}
}
