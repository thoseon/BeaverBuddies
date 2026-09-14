using System;
using Timberborn.Common;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class DialogBoxShower
{
	public class Builder
	{
		private readonly DialogBoxShower _dialogBoxShower;

		private readonly ILoc _loc;

		private readonly PanelStack _panelStack;

		private readonly VisualElement _root;

		private Action _confirmAction = EmptyCallback;

		private Action _cancelAction;

		private Action _infoAction;

		private string _confirmText;

		private string _cancelText;

		private string _infoText;

		public Builder(DialogBoxShower dialogBoxShower, ILoc loc, PanelStack panelStack, VisualElement root)
		{
			_dialogBoxShower = dialogBoxShower;
			_loc = loc;
			_panelStack = panelStack;
			_root = root;
		}

		public Builder SetMessage(string text)
		{
			_root.Q<Label>("Message").text = text;
			return this;
		}

		public Builder SetLocalizedMessage(string locKey)
		{
			return SetMessage(_loc.T(locKey));
		}

		public Builder SetConfirmButton(Action confirmAction)
		{
			_confirmAction = confirmAction;
			return this;
		}

		public Builder SetConfirmButton(Action confirmAction, string confirmText)
		{
			_confirmText = confirmText;
			return SetConfirmButton(confirmAction);
		}

		public Builder SetCancelButton(Action cancelAction)
		{
			_cancelAction = cancelAction;
			return this;
		}

		public Builder SetCancelButton(Action cancelAction, string cancelText)
		{
			_cancelText = cancelText;
			return SetCancelButton(cancelAction);
		}

		public Builder SetDefaultCancelButton()
		{
			return SetCancelButton(EmptyCallback);
		}

		public Builder SetDefaultCancelButton(string cancelText)
		{
			return SetCancelButton(EmptyCallback, cancelText);
		}

		public Builder SetInfoButton(Action infoAction, string infoText)
		{
			_infoAction = infoAction;
			_infoText = infoText;
			return this;
		}

		public Builder SetOffset(Vector2Int offsetInPixels)
		{
			OffsetBox(_root, offsetInPixels);
			return this;
		}

		public Builder SetMaxWidth(int maxWidth)
		{
			_root.Q<VisualElement>("Box").style.maxWidth = maxWidth;
			return this;
		}

		public Builder AddContent(VisualElement content)
		{
			_root.Q<VisualElement>("Content").Add(content);
			return this;
		}

		public DialogBox Show()
		{
			DialogBox dialogBox = CreateDialogBox();
			_dialogBoxShower.Show(dialogBox, hideTop: false);
			return dialogBox;
		}

		public void HideTopAndShow()
		{
			_dialogBoxShower.Show(CreateDialogBox(), hideTop: true);
		}

		private DialogBox CreateDialogBox()
		{
			DialogBox dialogBox = new DialogBox(_panelStack, _confirmAction, _cancelAction, _root);
			SetupButtons(dialogBox);
			return dialogBox;
		}

		private void SetupButtons(DialogBox dialogBox)
		{
			Button confirmButton = _root.Q<Button>("ConfirmButton");
			Button cancelButton = _root.Q<Button>("CancelButton");
			Button infoButton = _root.Q<Button>("InfoButton");
			SetupButtonsText(confirmButton, cancelButton, infoButton);
			SetupButtonsActions(dialogBox, confirmButton, cancelButton, infoButton);
		}

		private void SetupButtonsText(Button confirmButton, Button cancelButton, Button infoButton)
		{
			if (_confirmText == null)
			{
				string key = ((_cancelAction == null) ? CommonLocKeys.OKKey : CommonLocKeys.YesKey);
				_confirmText = _loc.T(key);
			}
			confirmButton.text = _confirmText;
			if (_cancelAction != null)
			{
				if (_cancelText == null)
				{
					_cancelText = _loc.T(CommonLocKeys.NoKey);
				}
				cancelButton.text = _cancelText;
			}
			if (!string.IsNullOrEmpty(_infoText))
			{
				infoButton.text = _infoText;
			}
		}

		private void SetupButtonsActions(DialogBox dialogBox, Button confirmButton, Button cancelButton, Button infoButton)
		{
			confirmButton.RegisterCallback<ClickEvent>(delegate
			{
				dialogBox.OnUIConfirmed();
			});
			if (_cancelAction != null)
			{
				cancelButton.RegisterCallback<ClickEvent>(delegate
				{
					dialogBox.OnUICancelled();
				});
			}
			else
			{
				cancelButton.ToggleDisplayStyle(visible: false);
			}
			if (_infoAction != null)
			{
				infoButton.RegisterCallback<ClickEvent>(delegate
				{
					_infoAction();
				});
			}
			else
			{
				infoButton.ToggleDisplayStyle(visible: false);
			}
		}

		private static void EmptyCallback()
		{
		}

		private static void OffsetBox(VisualElement box, Vector2Int offsetInPixels)
		{
			IStyle style = box.style;
			int x = offsetInPixels.x;
			if (x < 0)
			{
				style.right = Math.Abs(x);
			}
			else if (x > 0)
			{
				style.left = x;
			}
			int y = offsetInPixels.y;
			if (y < 0)
			{
				style.top = Math.Abs(y);
			}
			else if (y > 0)
			{
				style.bottom = y;
			}
		}
	}

	private readonly ILoc _loc;

	private readonly PanelStack _panelStack;

	private readonly VisualElementLoader _visualElementLoader;

	public DialogBoxShower(ILoc loc, PanelStack panelStack, VisualElementLoader visualElementLoader)
	{
		_loc = loc;
		_panelStack = panelStack;
		_visualElementLoader = visualElementLoader;
	}

	public Builder Create()
	{
		VisualElement root = _visualElementLoader.LoadVisualElement("Core/DialogBox");
		return new Builder(this, _loc, _panelStack, root);
	}

	private void Show(DialogBox dialogBox, bool hideTop)
	{
		if (hideTop)
		{
			_panelStack.HideAndPushDialog(dialogBox);
		}
		else
		{
			_panelStack.PushDialog(dialogBox);
		}
	}
}
