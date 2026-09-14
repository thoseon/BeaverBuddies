using System;
using Timberborn.CoreUI;
using Timberborn.ExperimentalModeSystem;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuScene;

internal class WelcomeScreenBox : ILoadableSingleton, IPanelController
{
	private readonly PanelStack _panelStack;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ExperimentalMode _experimentalMode;

	private VisualElement _root;

	private Action _onStart;

	public WelcomeScreenBox(PanelStack panelStack, VisualElementLoader visualElementLoader, ExperimentalMode experimentalMode)
	{
		_panelStack = panelStack;
		_visualElementLoader = visualElementLoader;
		_experimentalMode = experimentalMode;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("MainMenu/WelcomeScreenBox");
		_root.Q<Button>("Start").RegisterCallback<ClickEvent>(delegate
		{
			Start();
		});
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		Start();
		return true;
	}

	public void OnUICancelled()
	{
		Start();
	}

	public void Show(Action onStart)
	{
		if (_experimentalMode.IsExperimental)
		{
			_onStart = onStart;
			_panelStack.Push(this);
		}
		else
		{
			onStart?.Invoke();
		}
	}

	private void Start()
	{
		_panelStack.Pop(this);
		_onStart?.Invoke();
	}
}
