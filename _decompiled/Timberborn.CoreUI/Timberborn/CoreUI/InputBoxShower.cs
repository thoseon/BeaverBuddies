using System;
using Timberborn.InputSystem;
using Timberborn.Localization;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class InputBoxShower
{
	public class Builder
	{
		private readonly InputBoxShower _inputBoxShower;

		private readonly ILoc _loc;

		private readonly PanelStack _panelStack;

		private readonly InputService _inputService;

		private readonly VisualElement _root;

		private readonly TextField _input;

		private Action<string> _confirmAction = delegate
		{
		};

		public Builder(InputBoxShower inputBoxShower, ILoc loc, PanelStack panelStack, InputService inputService, VisualElement root, TextField input)
		{
			_inputBoxShower = inputBoxShower;
			_loc = loc;
			_panelStack = panelStack;
			_inputService = inputService;
			_root = root;
			_input = input;
		}

		public Builder SetDefaultValue(string value)
		{
			_input.value = value;
			return this;
		}

		public Builder SetLocalizedMessage(string locKey)
		{
			_root.Q<Label>("Message").text = _loc.T(locKey);
			return this;
		}

		public Builder SetConfirmButton(Action<string> confirmAction)
		{
			_confirmAction = confirmAction;
			return this;
		}

		public void Show()
		{
			_input.maxLength = CharacterLimit;
			InputBox inputBox = new InputBox(_panelStack, _confirmAction, _root, _input);
			_input.Q<TextElement>().SetConfirmCancelActions(_inputService, delegate
			{
				inputBox.OnUIConfirmed();
			}, delegate
			{
				inputBox.OnUICancelled();
			});
			_root.Q<Button>("ConfirmButton").RegisterCallback<ClickEvent>(delegate
			{
				inputBox.OnUIConfirmed();
			});
			_root.Q<Button>("CancelButton").RegisterCallback<ClickEvent>(delegate
			{
				inputBox.OnUICancelled();
			});
			_inputBoxShower.Show(inputBox);
			_input.Focus();
		}
	}

	private static readonly int CharacterLimit = 24;

	private readonly ILoc _loc;

	private readonly PanelStack _panelStack;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly InputService _inputService;

	public InputBoxShower(ILoc loc, PanelStack panelStack, VisualElementLoader visualElementLoader, InputService inputService)
	{
		_loc = loc;
		_panelStack = panelStack;
		_visualElementLoader = visualElementLoader;
		_inputService = inputService;
	}

	public Builder Create()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Core/InputBox");
		TextField input = visualElement.Q<TextField>("Input");
		return new Builder(this, _loc, _panelStack, _inputService, visualElement, input);
	}

	private void Show(InputBox inputBox)
	{
		_panelStack.PushDialog(inputBox);
	}
}
