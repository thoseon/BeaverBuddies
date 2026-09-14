using System;
using Timberborn.Common;
using Timberborn.InputSystem;
using UnityEngine.UIElements;

namespace Timberborn.InputSystemUI;

public class BindableToggle : IInputProcessor
{
	private readonly InputService _inputService;

	private readonly Toggle _toggle;

	private readonly string _bindingKey;

	private readonly Action<bool> _toggleAction;

	private readonly Func<bool> _valueGetter;

	private bool _isBound;

	public BindableToggle(InputService inputService, Toggle toggle, string bindingKey, Action<bool> toggleAction, Func<bool> valueGetter)
	{
		_inputService = inputService;
		_toggle = toggle;
		_bindingKey = bindingKey;
		_toggleAction = toggleAction;
		_valueGetter = valueGetter;
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
		_toggle.SetEnabled(value: true);
		Update();
	}

	public void Disable()
	{
		_toggle.SetEnabled(value: false);
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(_bindingKey) && _toggle.enabledInHierarchy)
		{
			bool flag = !_toggle.value;
			_toggleAction(flag);
			_toggle.SetValueWithoutNotify(flag);
			return true;
		}
		return false;
	}

	public void Update()
	{
		bool flag = _valueGetter();
		if (flag != _toggle.value)
		{
			_toggle.SetValueWithoutNotify(flag);
		}
	}
}
