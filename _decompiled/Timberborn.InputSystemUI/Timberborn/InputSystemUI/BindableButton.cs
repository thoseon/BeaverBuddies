using System;
using Timberborn.Common;
using Timberborn.InputSystem;
using UnityEngine.UIElements;

namespace Timberborn.InputSystemUI;

public class BindableButton : IInputProcessor
{
	private readonly InputService _inputService;

	private readonly VisualElement _button;

	private readonly string _bindingKey;

	private readonly Action _action;

	private readonly bool _blockInput;

	private bool _isBound;

	public BindableButton(InputService inputService, VisualElement button, string bindingKey, Action action, bool blockInput)
	{
		_inputService = inputService;
		_button = button;
		_bindingKey = bindingKey;
		_action = action;
		_blockInput = blockInput;
	}

	public void Bind()
	{
		Asserts.IsFalse(this, _isBound, "_isBound");
		_isBound = true;
		_inputService.AddInputProcessor(this);
	}

	public void Unbind()
	{
		_isBound = false;
		_inputService.RemoveInputProcessor(this);
	}

	public void Enable()
	{
		_button.SetEnabled(value: true);
	}

	public void Disable()
	{
		_button.SetEnabled(value: false);
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(_bindingKey) && _button.enabledInHierarchy)
		{
			_action();
			return _blockInput;
		}
		return false;
	}
}
