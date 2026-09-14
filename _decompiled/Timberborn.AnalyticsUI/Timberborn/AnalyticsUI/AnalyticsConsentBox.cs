using System;
using Timberborn.Analytics;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.WebNavigation;
using UnityEngine.UIElements;

namespace Timberborn.AnalyticsUI;

public class AnalyticsConsentBox : IPanelController, ILoadableSingleton
{
	private readonly PanelStack _panelStack;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly AnalyticsConsent _analyticsConsent;

	private readonly HyperlinkInitializer _hyperlinkInitializer;

	private readonly UrlOpener _urlOpener;

	private VisualElement _root;

	private Action _closedCallback;

	public AnalyticsConsentBox(PanelStack panelStack, VisualElementLoader visualElementLoader, AnalyticsConsent analyticsConsent, HyperlinkInitializer hyperlinkInitializer, UrlOpener urlOpener)
	{
		_panelStack = panelStack;
		_visualElementLoader = visualElementLoader;
		_analyticsConsent = analyticsConsent;
		_hyperlinkInitializer = hyperlinkInitializer;
		_urlOpener = urlOpener;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("MainMenu/AnalyticsConsentBox");
		_hyperlinkInitializer.Initialize(_root.Q<Label>("Info"), _urlOpener.OpenAnalyticsPrivacyPolicy);
		_root.Q<Button>("Agree").RegisterCallback<ClickEvent>(delegate
		{
			Agree();
		});
		_root.Q<Button>("Disagree").RegisterCallback<ClickEvent>(delegate
		{
			Disagree();
		});
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		return false;
	}

	public void OnUICancelled()
	{
	}

	public void Show(Action closedCallback)
	{
		if (!_analyticsConsent.WasConsentAsked)
		{
			_closedCallback = closedCallback;
			_panelStack.Push(this);
		}
		else
		{
			closedCallback();
		}
	}

	private void Agree()
	{
		_analyticsConsent.GiveConsent();
		Close();
	}

	private void Disagree()
	{
		_analyticsConsent.RemoveConsent();
		Close();
	}

	private void Close()
	{
		_panelStack.Pop(this);
		_closedCallback();
	}
}
