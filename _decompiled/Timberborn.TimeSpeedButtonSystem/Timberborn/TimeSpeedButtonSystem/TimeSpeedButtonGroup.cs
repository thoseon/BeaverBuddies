using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Timberborn.InputSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using UnityEngine.UIElements;

namespace Timberborn.TimeSpeedButtonSystem;

public class TimeSpeedButtonGroup : IUpdatableSingleton, IInputProcessor
{
	private static readonly string CustomButtonClass = "speed-button--custom";

	private static readonly string DecreaseSpeedKey = "DecreaseSpeed";

	private static readonly string IncreaseSpeedKey = "IncreaseSpeed";

	private readonly EventBus _eventBus;

	private readonly InputService _inputService;

	private readonly TimeSpeedButtonFactory _timeSpeedButtonFactory;

	private readonly List<TimeSpeedButton> _buttons = new List<TimeSpeedButton>();

	private Button _customSpeedButton;

	private float _previousSpeed = -1f;

	private Func<float> _currentSpeedGetter;

	private Action<int> _speedSetter;

	private bool _enabled;

	public TimeSpeedButtonGroup(EventBus eventBus, InputService inputService, TimeSpeedButtonFactory timeSpeedButtonFactory)
	{
		_eventBus = eventBus;
		_inputService = inputService;
		_timeSpeedButtonFactory = timeSpeedButtonFactory;
	}

	public void Initialize(IEnumerable<Button> buttons, Func<float> currentSpeedGetter, Action<int> speedSetter)
	{
		_currentSpeedGetter = currentSpeedGetter;
		_speedSetter = speedSetter;
		InitializeButtons(buttons);
		_eventBus.Register(this);
		_inputService.AddInputProcessor(this);
		_enabled = true;
	}

	public void UpdateSingleton()
	{
		if (_enabled)
		{
			float num = _currentSpeedGetter();
			if (_previousSpeed != num)
			{
				HighlightButton(num);
				_previousSpeed = num;
			}
		}
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(DecreaseSpeedKey))
		{
			DecreaseSpeedIfPossible();
		}
		else if (_inputService.IsKeyDown(IncreaseSpeedKey))
		{
			IncreaseSpeedIfPossible();
		}
		return false;
	}

	[OnEvent]
	public void OnSpeedLockChanged(SpeedLockChangedEvent speedLockChangedEvent)
	{
		foreach (TimeSpeedButton button in _buttons)
		{
			button.Button.SetEnabled(!speedLockChangedEvent.IsLocked);
		}
	}

	private void InitializeButtons(IEnumerable<Button> buttons)
	{
		int num = 0;
		foreach (Button button in buttons)
		{
			_buttons.Add(_timeSpeedButtonFactory.Create(button, num++, _speedSetter));
		}
		_customSpeedButton = _buttons.Last().Button;
	}

	private void DecreaseSpeedIfPossible()
	{
		TimeSpeedButton currentButton = GetCurrentButton();
		if (currentButton != null && currentButton != _buttons[0])
		{
			SetSpeed(_buttons.IndexOf(currentButton) - 1);
		}
	}

	private void IncreaseSpeedIfPossible()
	{
		TimeSpeedButton currentButton = GetCurrentButton();
		if (currentButton != null)
		{
			List<TimeSpeedButton> buttons = _buttons;
			if (currentButton != buttons[buttons.Count - 1])
			{
				SetSpeed(_buttons.IndexOf(currentButton) + 1);
			}
		}
	}

	private TimeSpeedButton GetCurrentButton()
	{
		float currentSpeed = _currentSpeedGetter();
		return _buttons.SingleOrDefault((TimeSpeedButton button) => (float)button.TimeSpeed == currentSpeed);
	}

	private void SetSpeed(int buttonIndex)
	{
		int timeSpeed = _buttons[buttonIndex].TimeSpeed;
		_speedSetter(timeSpeed);
	}

	private void HighlightButton(float speed)
	{
		TimeSpeedButton timeSpeedButton = _buttons.SingleOrDefault((TimeSpeedButton button) => (float)button.TimeSpeed == speed);
		if (timeSpeedButton != null)
		{
			timeSpeedButton.Highlight();
			_customSpeedButton.RemoveFromClassList(CustomButtonClass);
			_customSpeedButton.text = "";
		}
		else
		{
			_customSpeedButton.AddToClassList(CustomButtonClass);
			_customSpeedButton.text = "x" + speed.ToString(CultureInfo.InvariantCulture);
		}
		foreach (TimeSpeedButton button in _buttons)
		{
			if (button != timeSpeedButton)
			{
				button.Unhighlight();
			}
		}
	}
}
