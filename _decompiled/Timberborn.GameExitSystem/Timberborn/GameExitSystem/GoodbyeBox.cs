using System;
using Timberborn.CoreUI;
using Timberborn.WebNavigation;
using UnityEngine.UIElements;

namespace Timberborn.GameExitSystem;

public class GoodbyeBox : IPanelController
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly UrlOpener _urlOpener;

	private readonly Action _exitAction;

	public GoodbyeBox(VisualElementLoader visualElementLoader, PanelStack panelStack, UrlOpener urlOpener, Action action)
	{
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_urlOpener = urlOpener;
		_exitAction = action;
	}

	public VisualElement GetPanel()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/GoodbyeBox");
		visualElement.Q<Button>("Feedback").RegisterCallback<ClickEvent>(delegate
		{
			_urlOpener.OpenFeatureUpvote();
		});
		visualElement.Q<Button>("Exit").RegisterCallback<ClickEvent>(delegate
		{
			_exitAction();
		});
		visualElement.Q<Button>("CancelButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		return visualElement;
	}

	public bool OnUIConfirmed()
	{
		_exitAction();
		return true;
	}

	public void OnUICancelled()
	{
		_panelStack.Pop(this);
	}
}
