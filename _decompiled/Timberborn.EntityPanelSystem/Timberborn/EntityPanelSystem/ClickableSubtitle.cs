using System;

namespace Timberborn.EntityPanelSystem;

public readonly struct ClickableSubtitle
{
	public Action ClickAction { get; }

	public string Subtitle { get; }

	public string TooltipText { get; }

	public bool HasWarning { get; }

	public bool HasAction => ClickAction != null;

	private ClickableSubtitle(Action clickAction, string subtitle, string tooltipText, bool hasWarning)
	{
		ClickAction = clickAction;
		Subtitle = subtitle;
		TooltipText = tooltipText;
		HasWarning = hasWarning;
	}

	public static ClickableSubtitle Create(Action clickAction, string subtitle)
	{
		return new ClickableSubtitle(clickAction, subtitle, null, hasWarning: false);
	}

	public static ClickableSubtitle Create(Action clickAction, string subtitle, string tooltipText, bool isWarning)
	{
		return new ClickableSubtitle(clickAction, subtitle, tooltipText, isWarning);
	}

	public static ClickableSubtitle CreateEmpty()
	{
		return new ClickableSubtitle(null, null, null, hasWarning: false);
	}
}
