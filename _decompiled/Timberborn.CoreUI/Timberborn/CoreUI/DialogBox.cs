using System;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class DialogBox : IPanelController
{
	private readonly PanelStack _panelStack;

	private readonly Action _confirmButtonCallback;

	private readonly Action _cancelButtonCallback;

	private readonly VisualElement _root;

	private bool IsDialog => _cancelButtonCallback != null;

	public DialogBox(PanelStack panelStack, Action confirmButtonCallback, Action cancelButtonCallback, VisualElement root)
	{
		_confirmButtonCallback = confirmButtonCallback;
		_cancelButtonCallback = cancelButtonCallback;
		_panelStack = panelStack;
		_root = root;
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		Close();
		_confirmButtonCallback();
		return true;
	}

	public void OnUICancelled()
	{
		Close();
		if (IsDialog)
		{
			_cancelButtonCallback();
		}
		else
		{
			_confirmButtonCallback();
		}
	}

	public void Close()
	{
		_panelStack.Pop(this);
	}
}
