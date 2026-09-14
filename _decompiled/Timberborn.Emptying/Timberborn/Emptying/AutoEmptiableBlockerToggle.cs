namespace Timberborn.Emptying;

public class AutoEmptiableBlockerToggle
{
	private readonly AutoEmptiableBlocker _autoEmptiableBlocker;

	private bool _isBlocked;

	internal AutoEmptiableBlockerToggle(AutoEmptiableBlocker autoEmptiableBlocker)
	{
		_autoEmptiableBlocker = autoEmptiableBlocker;
	}

	public void Block()
	{
		if (!_isBlocked)
		{
			_autoEmptiableBlocker.IncrementBlockingToggles();
			_isBlocked = true;
		}
	}

	public void Unblock()
	{
		if (_isBlocked)
		{
			_autoEmptiableBlocker.DecrementBlockingToggles();
			_isBlocked = false;
		}
	}
}
