using Timberborn.CoreUI;
using Timberborn.KeyBindingSystemUI;
using Timberborn.TutorialSystem;
using UnityEngine.UIElements;

namespace Timberborn.TutorialSystemUI;

internal class TutorialStepViewFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly KeyBindingShortcutService _keyBindingShortcutService;

	private readonly FixedKeyBindingElementFactory _fixedKeyBindingElementFactory;

	public TutorialStepViewFactory(VisualElementLoader visualElementLoader, KeyBindingShortcutService keyBindingShortcutService, FixedKeyBindingElementFactory fixedKeyBindingElementFactory)
	{
		_visualElementLoader = visualElementLoader;
		_keyBindingShortcutService = keyBindingShortcutService;
		_fixedKeyBindingElementFactory = fixedKeyBindingElementFactory;
	}

	public TutorialStepView Create(TutorialStep tutorialStep, TutorialPanel parent)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/Tutorial/TutorialStepView");
		Label description = visualElement.Q<Label>("Description");
		if (tutorialStep.KeyBinding != null)
		{
			Label textElement = visualElement.Q<Label>("KeyBinding");
			_keyBindingShortcutService.CreateAny(textElement, tutorialStep.KeyBinding);
		}
		if (tutorialStep.FixedKeyBinding != null)
		{
			VisualElement visualElement2 = visualElement.Q<VisualElement>("FixedKeyBinding");
			visualElement2.ToggleDisplayStyle(visible: true);
			visualElement2.Add(_fixedKeyBindingElementFactory.Create(tutorialStep.FixedKeyBinding));
		}
		return new TutorialStepView(tutorialStep, parent, visualElement, description);
	}
}
