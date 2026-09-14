using System;
using System.Collections.Concurrent;
using System.IO;
using Timberborn.CoreUI;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.PlatformUtilities;
using Timberborn.RootProviders;
using Timberborn.SingletonSystem;
using Timberborn.Versioning;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.Console;

internal class ConsolePanel : IConsolePanel, IPriorityInputProcessor, ILoadableSingleton, IUnloadableSingleton, ILateUpdatableSingleton
{
	private static readonly int MaxCharacters = 20000;

	private static readonly string ToggleConsoleKey = "ToggleConsole";

	private static readonly string CollapsedClass = "console-panel--collapsed";

	private static readonly string ExpandedClass = "console-panel--expanded";

	private static readonly string CollapseLocKey = "Console.Collapse";

	private static readonly string ExpandLocKey = "Console.Expand";

	private readonly InputService _inputService;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly IExplorerOpener _explorerOpener;

	private readonly RootVisualElementProvider _rootVisualElementProvider;

	private VisualElement _root;

	private VisualElement _consolePanel;

	private Scroller _scroller;

	private TextField _textField;

	private Button _expandButton;

	private bool _isShown;

	private bool _justOpened;

	private bool _isExpanded;

	private bool _resetScroll;

	private readonly ConcurrentQueue<Log> _queuedLogs = new ConcurrentQueue<Log>();

	private static bool ShouldAutoOpenOnWarningOrError
	{
		get
		{
			if (!Application.isEditor)
			{
				return GameVersions.CurrentVersion.IsDevelopmentVersion;
			}
			return false;
		}
	}

	public ConsolePanel(InputService inputService, VisualElementLoader visualElementLoader, ILoc loc, IExplorerOpener explorerOpener, RootVisualElementProvider rootVisualElementProvider)
	{
		_inputService = inputService;
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_explorerOpener = explorerOpener;
		_rootVisualElementProvider = rootVisualElementProvider;
	}

	public void Load()
	{
		_root = _rootVisualElementProvider.Create("ConsolePanel", "Common/Console/ConsoleContainer", 10);
		_consolePanel = _visualElementLoader.LoadVisualElement("Common/Console/ConsolePanel");
		_textField = _consolePanel.Q<TextField>("TextField");
		_textField.Q<TextElement>().enableRichText = true;
		_scroller = _textField.Q<Scroller>();
		_expandButton = _consolePanel.Q<Button>("ExpandButton");
		_expandButton.RegisterCallback<ClickEvent>(delegate
		{
			ToggleExpand(!_isExpanded);
		});
		ToggleExpand(isExpanded: false);
		_consolePanel.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			Hide();
		});
		_consolePanel.Q<Button>("OpenDirectoryButton").RegisterCallback<ClickEvent>(OpenLogDirectory);
		_root.Q<VisualElement>("ConsoleContainer").Add(_consolePanel);
		_root.ToggleDisplayStyle(visible: false);
		_inputService.AddInputProcessor(this);
		if (ConsoleLogListener.AnyWarningOrError && ShouldAutoOpenOnWarningOrError)
		{
			Show();
		}
		ConsoleLogListener.OnFirstWarningOrErrorReceived += OnFirstWarningOrErrorReceived;
	}

	public void Unload()
	{
		ConsoleLogListener.OnFirstWarningOrErrorReceived -= OnFirstWarningOrErrorReceived;
		ConsoleLogListener.OnLogReceived -= OnLogReceived;
	}

	public void ProcessInput()
	{
		if (_inputService.IsKeyDown(ToggleConsoleKey))
		{
			if (!_isShown)
			{
				Show();
			}
			else
			{
				Hide();
			}
		}
	}

	public void LateUpdateSingleton()
	{
		if (_resetScroll)
		{
			ResetScrollPosition();
			_resetScroll = false;
		}
		_resetScroll = !_queuedLogs.IsEmpty && _root.IsDisplayed() && IsScrollAtBottom();
		if (!_justOpened)
		{
			Log result;
			while (_queuedLogs.TryDequeue(out result))
			{
				Add(result);
			}
		}
		else
		{
			_justOpened = false;
		}
	}

	public void Show()
	{
		if (!_isShown)
		{
			foreach (Log allLog in ConsoleLogListener.GetAllLogs())
			{
				_queuedLogs.Enqueue(allLog);
			}
			ConsoleLogListener.OnLogReceived += OnLogReceived;
			_justOpened = true;
			_root.ToggleDisplayStyle(visible: true);
			_isShown = true;
		}
	}

	private void ToggleExpand(bool isExpanded)
	{
		_isExpanded = isExpanded;
		_expandButton.text = (_isExpanded ? _loc.T(CollapseLocKey) : _loc.T(ExpandLocKey));
		_consolePanel.EnableInClassList(CollapsedClass, !_isExpanded);
		_consolePanel.EnableInClassList(ExpandedClass, _isExpanded);
	}

	private void OnLogReceived(object sender, Log log)
	{
		_queuedLogs.Enqueue(log);
	}

	private void OnFirstWarningOrErrorReceived(object sender, Log e)
	{
		if (ShouldAutoOpenOnWarningOrError)
		{
			Show();
		}
	}

	private void Hide()
	{
		if (_isShown)
		{
			ConsoleLogListener.OnLogReceived -= OnLogReceived;
			_queuedLogs.Clear();
			_root.ToggleDisplayStyle(visible: false);
			_textField.value = "";
			_isShown = false;
		}
	}

	private bool IsScrollAtBottom()
	{
		return Math.Abs(_scroller.value - _scroller.highValue) < 15f;
	}

	private void Add(Log log)
	{
		string text = ColorUtility.ToHtmlStringRGB(GetLogColor(log));
		_textField.value = Trim(_textField.value + "<color=#" + text + ">" + log.Message + "</color>" + Environment.NewLine);
	}

	private static Color GetLogColor(Log log)
	{
		return log.LogType switch
		{
			LogType.Error => Color.red, 
			LogType.Assert => Color.red, 
			LogType.Warning => Color.yellow, 
			LogType.Log => Color.white, 
			LogType.Exception => Color.red, 
			_ => throw new ArgumentOutOfRangeException("LogType", log.LogType, null), 
		};
	}

	private static string Trim(string text)
	{
		int length = text.Length;
		if (length > MaxCharacters)
		{
			text = text.Substring(length - MaxCharacters, MaxCharacters);
			int num = text.IndexOf('\n', StringComparison.Ordinal);
			string text2 = text;
			int num2 = num + 1;
			text = text2.Substring(num2, text2.Length - num2);
		}
		return text;
	}

	private void ResetScrollPosition()
	{
		_scroller.value = _scroller.highValue;
	}

	private void OpenLogDirectory(ClickEvent evt)
	{
		string directoryName = Path.GetDirectoryName(Application.consoleLogPath);
		_explorerOpener.OpenDirectory(directoryName);
	}
}
