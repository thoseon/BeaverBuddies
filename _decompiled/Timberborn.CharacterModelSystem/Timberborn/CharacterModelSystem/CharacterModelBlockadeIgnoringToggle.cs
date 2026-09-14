namespace Timberborn.CharacterModelSystem;

public class CharacterModelBlockadeIgnoringToggle
{
	private readonly CharacterModel _characterModel;

	private bool _isBlocked;

	internal CharacterModelBlockadeIgnoringToggle(CharacterModel characterModel)
	{
		_characterModel = characterModel;
	}

	public void Block()
	{
		if (!_isBlocked)
		{
			_characterModel.IncrementBlockageIgnoringToggles();
			_isBlocked = true;
		}
	}

	public void Unblock()
	{
		if (_isBlocked)
		{
			_characterModel.DecrementBlockageIgnoringToggles();
			_isBlocked = false;
		}
	}
}
