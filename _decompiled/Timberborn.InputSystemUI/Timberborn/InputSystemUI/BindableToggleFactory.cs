using System;
using Timberborn.InputSystem;
using UnityEngine.UIElements;

namespace Timberborn.InputSystemUI;

public class BindableToggleFactory
{
	private readonly InputService _inputService;

	public BindableToggleFactory(InputService inputService)
	{
		_inputService = inputService;
	}

	public BindableToggle Create(Toggle toggle, string bindingKey, Action<bool> toggleAction, Func<bool> valueGetter)
	{
		toggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> evt)
		{
			toggleAction(evt.newValue);
		});
		return new BindableToggle(_inputService, toggle, bindingKey, toggleAction, valueGetter);
	}

	public BindableToggle CreateAndBind(Toggle toggle, string bindingKey, Action<bool> toggleAction, Func<bool> valueGetter)
	{
		BindableToggle bindableToggle = Create(toggle, bindingKey, toggleAction, valueGetter);
		bindableToggle.Bind();
		bindableToggle.Update();
		return bindableToggle;
	}
}
