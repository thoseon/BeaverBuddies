using Timberborn.InputSystem;
using UnityEngine.UIElements;

namespace Timberborn.CoreUI;

internal class TextElementInitializer : IVisualElementInitializer
{
	private readonly InputBlocker _inputBlocker;

	public TextElementInitializer(InputBlocker inputBlocker)
	{
		_inputBlocker = inputBlocker;
	}

	public void InitializeVisualElement(VisualElement visualElement)
	{
		if (visualElement is TextField textField)
		{
			if (!textField.isReadOnly)
			{
				goto IL_003a;
			}
		}
		else if (visualElement is IntegerField integerField)
		{
			if (!integerField.isReadOnly)
			{
				goto IL_003a;
			}
		}
		else if (visualElement is FloatField { isReadOnly: false })
		{
			goto IL_003a;
		}
		bool flag = false;
		goto IL_0040;
		IL_0040:
		if (flag)
		{
			TextElement textElement = visualElement.Q<TextElement>();
			textElement.RegisterCallback<FocusInEvent>(OnFocusIn);
			textElement.RegisterCallback<FocusOutEvent>(OnFocusOut);
		}
		return;
		IL_003a:
		flag = true;
		goto IL_0040;
	}

	private void OnFocusIn(FocusInEvent evt)
	{
		_inputBlocker.Block();
	}

	private void OnFocusOut(FocusOutEvent evt)
	{
		_inputBlocker.Unblock();
	}
}
