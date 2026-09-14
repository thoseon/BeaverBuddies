using System;
using Timberborn.InputSystem;
using UnityEngine.UIElements;

namespace Timberborn.InputSystemUI;

public class BindableButtonFactory
{
	private readonly InputService _inputService;

	public BindableButtonFactory(InputService inputService)
	{
		_inputService = inputService;
	}

	public BindableButton Create(VisualElement button, string bindingKey, Action action, bool blockInput = true)
	{
		button.RegisterCallback<ClickEvent>(delegate
		{
			action();
		});
		return new BindableButton(_inputService, button, bindingKey, action, blockInput);
	}

	public BindableButton CreateAndBind(VisualElement button, string bindingKey, Action action)
	{
		BindableButton bindableButton = Create(button, bindingKey, action);
		bindableButton.Bind();
		return bindableButton;
	}
}
