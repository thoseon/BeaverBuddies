using System;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.FileBrowsing;

public class FileBrowser : ILoadableSingleton, IPanelController
{
	private static readonly string LastOpenedPathKey = "FileBrowser.LastOpenedPath";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly DirectoryListView _directoryListView;

	private readonly ILoc _loc;

	private VisualElement _root;

	private TextField _pathField;

	private Label _tipLabel;

	private Action<string> _openFileCallback;

	private string _focusInPath;

	public FileBrowser(VisualElementLoader visualElementLoader, PanelStack panelStack, DirectoryListView directoryListView, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_directoryListView = directoryListView;
		_loc = loc;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/FileBrowser");
		_pathField = _root.Q<TextField>("PathField");
		_tipLabel = _root.Q<Label>("Tip");
		_root.Q<Button>("UpwardButton").RegisterCallback<ClickEvent>(delegate
		{
			_directoryListView.GoUpward();
		});
		_root.Q<Button>("OpenButton").RegisterCallback<ClickEvent>(delegate
		{
			OpenCurrentSelection();
		});
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			Close();
		});
		_directoryListView.Initialize(_root);
		_directoryListView.DirectoryChanged += OnDirectoryChanged;
		_directoryListView.EntryDoubleClicked += delegate(object _, DiskSystemEntry diskSystemEntry)
		{
			OpenDiskSystemEntry(diskSystemEntry);
		};
		_pathField.Q<TextElement>().RegisterCallback<FocusInEvent>(OnFocusIn);
		_pathField.Q<TextElement>().RegisterCallback<FocusOutEvent>(OnFocusOut);
	}

	public void Open(Action<string> openFileCallback, FileFilter fileFilter, string tipLocKey)
	{
		Asserts.FieldIsNull(this, _openFileCallback, "_openFileCallback");
		_openFileCallback = openFileCallback;
		_panelStack.PushDialog(this);
		_directoryListView.SetFileFilter(fileFilter);
		UpdateTip(tipLocKey);
		OpenInitialDirectory();
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		OpenCurrentSelection();
		return true;
	}

	public void OnUICancelled()
	{
		Close();
	}

	private void OnFocusIn(FocusInEvent evt)
	{
		_focusInPath = _pathField.value;
	}

	private void OnFocusOut(FocusOutEvent evt)
	{
		if (_focusInPath != _pathField.value)
		{
			ProcessPathFieldPath();
		}
	}

	private void OpenCurrentSelection()
	{
		if (_directoryListView.TryGetSelectedDiskSystemEntry(out var diskSystemEntry))
		{
			OpenDiskSystemEntry(diskSystemEntry);
		}
	}

	private void OpenDiskSystemEntry(DiskSystemEntry diskSystemEntry)
	{
		if (diskSystemEntry.IsDirectory)
		{
			_directoryListView.TryOpenDirectory(diskSystemEntry.Path);
			return;
		}
		_openFileCallback?.Invoke(diskSystemEntry.Path);
		Close();
	}

	private void Close()
	{
		_panelStack.Pop(this);
		_openFileCallback = null;
		_directoryListView.Clear();
	}

	private void ProcessPathFieldPath()
	{
		DiskSystemEntry diskSystemEntry = DiskSystemEntry.Create(_pathField.value);
		if (diskSystemEntry.Exists)
		{
			OpenDiskSystemEntry(diskSystemEntry);
		}
		else
		{
			_pathField.SetValueWithoutNotify(_directoryListView.CurrentPath);
		}
	}

	private void OnDirectoryChanged(object sender, EventArgs eventArgs)
	{
		_pathField.SetValueWithoutNotify(_directoryListView.CurrentPath);
		PlayerPrefs.SetString(LastOpenedPathKey, _directoryListView.CurrentPath);
	}

	private void UpdateTip(string tipLocKey)
	{
		if (string.IsNullOrEmpty(tipLocKey))
		{
			_tipLabel.ToggleDisplayStyle(visible: false);
			return;
		}
		_tipLabel.text = _loc.T(tipLocKey);
		_tipLabel.ToggleDisplayStyle(visible: true);
	}

	private void OpenInitialDirectory()
	{
		if (!TryOpenLastDirectory() && !_directoryListView.TryOpenDirectory(UserDataFolder.Folder))
		{
			throw new InvalidOperationException("Could not open user data folder");
		}
	}

	private bool TryOpenLastDirectory()
	{
		if (PlayerPrefs.HasKey(LastOpenedPathKey))
		{
			return _directoryListView.TryOpenDirectory(PlayerPrefs.GetString(LastOpenedPathKey));
		}
		return false;
	}
}
