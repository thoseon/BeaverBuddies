using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;

namespace Timberborn.ToolButtonSystem;

public class ToolbarButtonRetriever
{
	public bool TryGetNextVisibleButton(IReadOnlyList<IToolbarButton> buttons, out IToolbarButton nextButton)
	{
		int num = GetActiveButtonIndex(buttons) + 1;
		for (int i = num; i < buttons.Count; i++)
		{
			if (buttons[i].IsVisible)
			{
				nextButton = buttons[i];
				return true;
			}
		}
		for (int j = 0; j < num; j++)
		{
			if (buttons[j].IsVisible)
			{
				nextButton = buttons[j];
				return true;
			}
		}
		nextButton = null;
		return false;
	}

	public bool TryGetPreviousVisibleButton(IReadOnlyList<IToolbarButton> buttons, out IToolbarButton previousButton)
	{
		int num = GetActiveButtonIndex(buttons) - 1;
		for (int num2 = num; num2 >= 0; num2--)
		{
			if (buttons[num2].IsVisible)
			{
				previousButton = buttons[num2];
				return true;
			}
		}
		for (int num3 = buttons.Count - 1; num3 > num; num3--)
		{
			if (buttons[num3].IsVisible)
			{
				previousButton = buttons[num3];
				return true;
			}
		}
		previousButton = null;
		return false;
	}

	private static int GetActiveButtonIndex(IReadOnlyList<IToolbarButton> buttons)
	{
		IToolbarButton obj = buttons.LastOrDefault((IToolbarButton button) => button.IsActive);
		return buttons.IndexOf(obj);
	}
}
