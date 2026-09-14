using System;
using Timberborn.CoreUI;
using Timberborn.ErrorReporting;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.ErrorReportingUI;

internal class CrashBox : ILoadableSingleton, IUnloadableSingleton
{
	private readonly RootObjectProvider _rootObjectProvider;

	private readonly RootVisualElementProvider _rootVisualElementProvider;

	private GameObject _root;

	private VisualElement _rootVisualElement;

	public CrashBox(RootObjectProvider rootObjectProvider, RootVisualElementProvider rootVisualElementProvider)
	{
		_rootObjectProvider = rootObjectProvider;
		_rootVisualElementProvider = rootVisualElementProvider;
	}

	public void Load()
	{
		_root = _rootObjectProvider.CreateRootObject("CrashBox");
		_rootVisualElement = _rootVisualElementProvider.Create(_root, "Common/CrashBox", 10);
		_rootVisualElement.ToggleDisplayStyle(visible: false);
		ExceptionListener.FirstUncaughtException += OnFirstUncaughtException;
	}

	public void Unload()
	{
		ExceptionListener.FirstUncaughtException -= OnFirstUncaughtException;
	}

	private void OnFirstUncaughtException(object sender, EventArgs e)
	{
		if (!CrashSceneLoader.Enabled)
		{
			_rootVisualElement.ToggleDisplayStyle(visible: true);
			_root.SetActive(value: true);
		}
	}
}
