using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.TitleScreenUI;

public class TitleScreen
{
	private static readonly string TitleScreenName = "MainMenu/TitleScreen";

	private static readonly string ContainerName = "TitleScreenContent";

	private static readonly string RootName = "TitleScreen";

	private static readonly string BackgroundClass = "title-screen-background";

	private readonly PanelStack _panelStack;

	private readonly TitleScreenFooter _titleScreenFooter;

	private readonly VisualElementInitializer _visualElementInitializer;

	private VisualElement _root;

	public TitleScreen(PanelStack panelStack, TitleScreenFooter titleScreenFooter, VisualElementInitializer visualElementInitializer)
	{
		_panelStack = panelStack;
		_titleScreenFooter = titleScreenFooter;
		_visualElementInitializer = visualElementInitializer;
	}

	public void Initialize()
	{
		VisualElement visualElement = _panelStack.Initialize(TitleScreenName, ContainerName);
		_titleScreenFooter.Initialize(visualElement);
		_visualElementInitializer.InitializeVisualElement(visualElement);
		_root = visualElement.Q<VisualElement>(RootName);
	}

	public void HideBackground()
	{
		_root.RemoveFromClassList(BackgroundClass);
	}

	public void ShowBackground()
	{
		_root.AddToClassList(BackgroundClass);
	}
}
