using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.EntityPanelSystem;

public class DebugFragmentFactory
{
	private static readonly string MarginClass = "debug-fragment--margin";

	private readonly VisualElementLoader _visualElementLoader;

	public DebugFragmentFactory(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement Create(string title)
	{
		VisualElement rootAndInitialize = GetRootAndInitialize(title);
		rootAndInitialize.Q<VisualElement>("Buttons").ToggleDisplayStyle(visible: false);
		return rootAndInitialize;
	}

	public VisualElement Create(string title, DebugFragmentButton debugFragmentButton)
	{
		VisualElement rootAndInitialize = GetRootAndInitialize(title);
		VisualElement visualElement = rootAndInitialize.Q<VisualElement>("Buttons");
		visualElement.AddToClassList(MarginClass);
		CreateButton(debugFragmentButton, visualElement);
		return rootAndInitialize;
	}

	public VisualElement Create(DebugFragmentButton debugFragmentButton)
	{
		VisualElement rootAndInitialize = GetRootAndInitialize();
		VisualElement root = rootAndInitialize.Q<VisualElement>("Buttons");
		CreateButton(debugFragmentButton, root);
		return rootAndInitialize;
	}

	public VisualElement Create(params DebugFragmentButton[] debugFragmentButtons)
	{
		VisualElement rootAndInitialize = GetRootAndInitialize();
		VisualElement root = rootAndInitialize.Q<VisualElement>("Buttons");
		for (int i = 0; i < debugFragmentButtons.Length; i++)
		{
			Button button = CreateButton(debugFragmentButtons[i], root);
			if (i > 0)
			{
				button.AddToClassList(MarginClass);
			}
		}
		return rootAndInitialize;
	}

	private VisualElement GetRootAndInitialize(string title = null)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/EntityPanel/DebugFragment");
		visualElement.Q<Label>("Title").text = title;
		visualElement.Q<VisualElement>("TitleWrapper").ToggleDisplayStyle(title != null);
		InitializeCallbacks(visualElement);
		ToggleVisibility(visualElement, visible: false);
		return visualElement;
	}

	private Button CreateButton(DebugFragmentButton debugFragmentButton, VisualElement root)
	{
		Button button = (Button)_visualElementLoader.LoadVisualElement("Game/EntityPanel/DebugButton");
		root.Add(button);
		button.RegisterCallback<ClickEvent>(delegate
		{
			debugFragmentButton.Action();
		});
		button.text = debugFragmentButton.Text;
		return button;
	}

	private static void InitializeCallbacks(VisualElement root)
	{
		root.Q<Button>("Show").RegisterCallback<ClickEvent>(delegate
		{
			ToggleVisibility(root, visible: true);
		});
		root.Q<Button>("Hide").RegisterCallback<ClickEvent>(delegate
		{
			ToggleVisibility(root, visible: false);
		});
	}

	private static void ToggleVisibility(VisualElement root, bool visible)
	{
		root.Q<Button>("Show").ToggleDisplayStyle(!visible);
		root.Q<Button>("Hide").ToggleDisplayStyle(visible);
		root.Q<Label>("Text").ToggleDisplayStyle(visible);
		root.Q<VisualElement>("Content").ToggleDisplayStyle(visible);
	}
}
