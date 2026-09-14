using System;
using Timberborn.InputSystem;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class AlternateClickable
{
	private static readonly string AlternateClass = "clickable--alternate";

	private static readonly string AlternateClickableActionKey = "AlternateClickableAction";

	private readonly InputService _inputService;

	private readonly Action _mainAction;

	private readonly Action _alternateAction;

	public VisualElement Root { get; }

	private bool IsAlternating => _inputService.IsKeyHeld(AlternateClickableActionKey);

	public AlternateClickable(InputService inputService, VisualElement root, Action mainAction, Action alternateAction)
	{
		_inputService = inputService;
		Root = root;
		_mainAction = mainAction;
		_alternateAction = alternateAction;
	}

	public void Update()
	{
		Root.EnableInClassList(AlternateClass, IsAlternating);
	}

	public void OnClick(ClickEvent evt)
	{
		if (IsAlternating)
		{
			_alternateAction();
		}
		else
		{
			_mainAction();
		}
	}
}
