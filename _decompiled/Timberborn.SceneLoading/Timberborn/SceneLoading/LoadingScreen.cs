using System;
using Timberborn.Localization;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.SceneLoading;

public class LoadingScreen : ILoadableSingleton
{
	private static readonly string LoadingLocKey = "Core.Loading";

	private readonly ILoc _loc;

	private readonly RootObjectProvider _rootObjectProvider;

	private readonly RootVisualElementProvider _rootVisualElementProvider;

	private VisualElement _root;

	public event EventHandler LoadingScreenEnabled;

	public event EventHandler LoadingScreenDisabled;

	public LoadingScreen(ILoc loc, RootObjectProvider rootObjectProvider, RootVisualElementProvider rootVisualElementProvider)
	{
		_loc = loc;
		_rootObjectProvider = rootObjectProvider;
		_rootVisualElementProvider = rootVisualElementProvider;
	}

	public void Load()
	{
		GameObject gameObject = _rootObjectProvider.CreateRootObject("LoadingScreen");
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		_root = _rootVisualElementProvider.Create(gameObject, "LoadingScreen/LoadingScreen", 10000, "UI/Views/LoadingScreen/LoadingScreenPanelSettings");
		Hide();
	}

	public void Enable(string tip)
	{
		Show();
		_root.Q<Label>("LoadingLabel").text = _loc.T(LoadingLocKey);
		VisualElement visualElement = _root.Q<VisualElement>("TipWrapper");
		Label label = _root.Q<Label>("TipText");
		if (string.IsNullOrEmpty(tip))
		{
			visualElement.style.display = DisplayStyle.None;
		}
		else
		{
			label.text = tip;
			visualElement.style.display = DisplayStyle.Flex;
		}
		LoadingScreenEnabled?.Invoke(this, EventArgs.Empty);
	}

	public void Disable()
	{
		LoadingScreenDisabled?.Invoke(this, EventArgs.Empty);
		Hide();
	}

	private void Show()
	{
		_root.style.display = DisplayStyle.Flex;
	}

	private void Hide()
	{
		_root.style.display = DisplayStyle.None;
	}
}
