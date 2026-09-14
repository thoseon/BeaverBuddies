using Timberborn.ToolButtonSystem;
using Timberborn.TutorialSystem;
using UnityEngine.UIElements;

namespace Timberborn.TutorialSystemUI;

internal class TutorialStepView
{
	private static readonly string FinishedClassKey = "tutorial-step-view--finished";

	private static readonly string TutorialToolClassKey = "tutorial-tool--highlighted";

	private readonly TutorialStep _tutorialStep;

	private readonly TutorialPanel _parent;

	private readonly Label _description;

	public bool IsAchieved { get; private set; }

	public VisualElement Root { get; }

	public TutorialStepView(TutorialStep tutorialStep, TutorialPanel parent, VisualElement root, Label description)
	{
		_tutorialStep = tutorialStep;
		_parent = parent;
		Root = root;
		_description = description;
		UpdateDescription();
	}

	public void Update()
	{
		IsAchieved = _tutorialStep.Step.Achieved();
		UpdateStyle();
		UpdateDescription();
		HighlightAssociatedTools();
	}

	public void UnhighlightAssociatedTools()
	{
		HighlightAssociatedTools(forceHide: true);
	}

	private void UpdateStyle()
	{
		Root.EnableInClassList(FinishedClassKey, IsAchieved);
	}

	private void UpdateDescription()
	{
		_description.text = _tutorialStep.Step.Description();
	}

	private void HighlightAssociatedTools(bool forceHide = false)
	{
		int num;
		if (_parent.IsVisible && !forceHide && !IsAchieved)
		{
			ITutorialStepWithTool obj = _tutorialStep.Step as ITutorialStepWithTool;
			if (obj == null || obj.KeepBlinking)
			{
				num = (HighlightTimer.IsTimeForPulsingHighlight() ? 1 : 0);
				goto IL_003e;
			}
		}
		num = 0;
		goto IL_003e;
		IL_003e:
		bool flag = (byte)num != 0;
		foreach (ToolButton toolButton in _tutorialStep.ToolButtons)
		{
			ToggleHighlight(toolButton.Root, flag);
		}
		ToolGroupButton toolGroupButton = _tutorialStep.ToolGroupButton;
		if (toolGroupButton != null)
		{
			ToggleHighlight(toolGroupButton.Root, flag);
		}
		_tutorialStep.Highlight?.Invoke(flag);
	}

	private static void ToggleHighlight(VisualElement root, bool state)
	{
		root.EnableInClassList(TutorialToolClassKey, state);
	}
}
