using System.Collections.Generic;
using System.Text;
using Timberborn.CoreUI;
using Timberborn.ErrorReporting;
using Timberborn.Localization;
using Timberborn.MainMenuSceneLoading;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.ErrorReportingUI;

internal class LoadingIssuePanel : ILoadableSingleton, IPanelController, IPanelBlocker
{
	private readonly ILoadingIssueService _loadingIssueService;

	private readonly PanelStack _panelStack;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly MainMenuSceneLoader _mainMenuSceneLoader;

	private readonly EventBus _eventBus;

	private readonly ILoc _loc;

	public LoadingIssuePanel(ILoadingIssueService loadingIssueService, PanelStack panelStack, VisualElementLoader visualElementLoader, MainMenuSceneLoader mainMenuSceneLoader, EventBus eventBus, ILoc loc)
	{
		_loadingIssueService = loadingIssueService;
		_panelStack = panelStack;
		_visualElementLoader = visualElementLoader;
		_mainMenuSceneLoader = mainMenuSceneLoader;
		_eventBus = eventBus;
		_loc = loc;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void ShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		if (_loadingIssueService.HasAnyIssues)
		{
			_panelStack.PushOverlay(this);
		}
		_eventBus.Unregister((object)this);
	}

	public VisualElement GetPanel()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/LoadingIssuePanel");
		visualElement.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		visualElement.Q<Button>("ContinuePlaying").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		visualElement.Q<Button>("ExitToMenu").RegisterCallback<ClickEvent>(delegate
		{
			_mainMenuSceneLoader.OpenMainMenu();
		});
		visualElement.Q<TextField>("Issues").value = GetText();
		return visualElement;
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
		_panelStack.Pop(this);
	}

	private string GetText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<string> list = new List<string>();
		foreach (var (issue, count) in _loadingIssueService.GetIssues())
		{
			list.Add(BuildIssueText(stringBuilder, issue, count));
			stringBuilder.Clear();
		}
		list.Sort();
		foreach (string item in list)
		{
			stringBuilder.AppendLine(item);
		}
		return stringBuilder.ToString();
	}

	private string BuildIssueText(StringBuilder stringBuilder, LoadingIssueMessage issue, int count)
	{
		stringBuilder.Append(SpecialStrings.RowStarter);
		stringBuilder.Append((issue.MessageParam != null) ? _loc.T(issue.MessageLocKey, GetParamText(issue)) : _loc.T(issue.MessageLocKey));
		if (count > 1)
		{
			stringBuilder.Append($" ({count})");
		}
		return stringBuilder.ToString();
	}

	private string GetParamText(LoadingIssueMessage issue)
	{
		if (!issue.ParamIsLocKey)
		{
			return issue.MessageParam;
		}
		return _loc.T(issue.MessageParam);
	}
}
