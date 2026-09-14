using System;

namespace Timberborn.EntityPanelSystem;

public readonly struct DebugFragmentButton
{
	public Action Action { get; }

	public string Text { get; }

	public DebugFragmentButton(Action action, string text)
	{
		Action = action;
		Text = text;
	}
}
