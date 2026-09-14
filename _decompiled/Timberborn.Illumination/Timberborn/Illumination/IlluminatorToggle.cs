namespace Timberborn.Illumination;

public class IlluminatorToggle
{
	private readonly Illuminator _illuminator;

	private bool _isOn;

	private bool _isDisabled;

	internal IlluminatorToggle(Illuminator illuminator)
	{
		_illuminator = illuminator;
	}

	public void TurnOn()
	{
		if (!_isOn)
		{
			_illuminator.IncrementTurnedOnToggles();
			_isOn = true;
		}
	}

	public void TurnOff()
	{
		if (_isOn)
		{
			_illuminator.DecrementTurnedOnToggles();
			_isOn = false;
		}
	}

	public void Disable()
	{
		if (!_isDisabled)
		{
			_illuminator.IncrementDisabledToggles();
			_isDisabled = true;
		}
	}

	public void Enable()
	{
		if (_isDisabled)
		{
			_illuminator.DecrementDisabledToggles();
			_isDisabled = false;
		}
	}

	public void Toggle(bool value)
	{
		if (value)
		{
			TurnOn();
		}
		else
		{
			TurnOff();
		}
	}

	public void Toggle()
	{
		Toggle(!_isOn);
	}
}
