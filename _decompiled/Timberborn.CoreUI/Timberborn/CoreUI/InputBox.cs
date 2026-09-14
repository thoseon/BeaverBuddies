using System;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

public class InputBox : IPanelController
{
	private readonly PanelStack _panelStack;

	private readonly Action<string> _confirmButtonCallback;

	private readonly VisualElement _root;

	private readonly TextField _input;

	public InputBox(PanelStack panelStack, Action<string> confirmButtonCallback, VisualElement root, TextField input)
	{
		_confirmButtonCallback = confirmButtonCallback;
		_panelStack = panelStack;
		_root = root;
		_input = input;
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		_panelStack.Pop(this);
		_confirmButtonCallback(_input.text);
		return true;
	}

	public void OnUICancelled()
	{
		_panelStack.Pop(this);
	}
}
