using System;
using Timberborn.InputSystem;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class AlternateClickableFactory
{
	private readonly InputService _inputService;

	public AlternateClickableFactory(InputService inputService)
	{
		_inputService = inputService;
	}

	public AlternateClickable Create(VisualElement visualElement, Action mainAction, Action alternateAction)
	{
		AlternateClickable alternateClickable = new AlternateClickable(_inputService, visualElement, mainAction, alternateAction);
		visualElement.RegisterCallback<ClickEvent>(alternateClickable.OnClick);
		return alternateClickable;
	}
}
