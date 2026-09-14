using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Timberborn.Common;
using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.FileBrowsing;

public class DirectoryListView
{
	private static readonly string NoPermissionKey = "FileBrowser.NoPermission";

	private readonly DiskSystemEntryElementFactory _diskSystemEntryElementFactory;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly List<DiskSystemEntry> _diskSystemEntries = new List<DiskSystemEntry>();

	private FileFilter _fileFilter;

	private ListView _diskSystemEntryView;

	private DiskSystemEntry _currentDirectory;

	public string CurrentPath => _currentDirectory.Path;

	public event EventHandler DirectoryChanged;

	public event EventHandler<DiskSystemEntry> EntryDoubleClicked;

	public DirectoryListView(DiskSystemEntryElementFactory diskSystemEntryElementFactory, DialogBoxShower dialogBoxShower)
	{
		_diskSystemEntryElementFactory = diskSystemEntryElementFactory;
		_dialogBoxShower = dialogBoxShower;
	}

	public void Initialize(VisualElement root)
	{
		Asserts.FieldIsNull(this, _diskSystemEntryView, "_diskSystemEntryView");
		_diskSystemEntryView = root.Q<ListView>("DiskSystemEntries");
		_diskSystemEntryView.makeItem = () => _diskSystemEntryElementFactory.Create(OnDiskSystemEntryClicked);
		_diskSystemEntryView.bindItem = delegate(VisualElement ve, int i)
		{
			_diskSystemEntryElementFactory.Bind(ve, _diskSystemEntries[i], _fileFilter);
		};
		_diskSystemEntryView.itemsSource = _diskSystemEntries;
		_diskSystemEntryView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
	}

	public void SetFileFilter(FileFilter fileFilter)
	{
		_fileFilter = fileFilter;
	}

	public void Clear()
	{
		_diskSystemEntries.Clear();
		_fileFilter = null;
	}

	public bool TryGetSelectedDiskSystemEntry(out DiskSystemEntry diskSystemEntry)
	{
		int selectedIndex = _diskSystemEntryView.selectedIndex;
		if (selectedIndex >= 0)
		{
			diskSystemEntry = _diskSystemEntries[selectedIndex];
			return true;
		}
		diskSystemEntry = default(DiskSystemEntry);
		return false;
	}

	public void GoUpward()
	{
		TryOpenDirectory(_currentDirectory.Parent);
	}

	public bool TryOpenDirectory(string path)
	{
		try
		{
			DiskSystemEntry diskSystemEntry = DiskSystemEntry.Create(path);
			if (diskSystemEntry.Exists)
			{
				OpenDirectory(diskSystemEntry);
				return true;
			}
		}
		catch (UnauthorizedAccessException)
		{
			ShowNoPermissionDialog();
		}
		return false;
	}

	private void OnDiskSystemEntryClicked(ClickEvent evt)
	{
		if (evt.clickCount > 1 && TryGetSelectedDiskSystemEntry(out var diskSystemEntry))
		{
			EntryDoubleClicked?.Invoke(this, diskSystemEntry);
		}
	}

	private void OpenDirectory(DiskSystemEntry diskSystemEntry)
	{
		_diskSystemEntries.Clear();
		_diskSystemEntries.AddRange(GetChildren(diskSystemEntry));
		_diskSystemEntryView.ClearSelection();
		_diskSystemEntryView.RefreshItems();
		_diskSystemEntryView.ScrollToItem(0);
		_currentDirectory = diskSystemEntry;
		DirectoryChanged?.Invoke(this, EventArgs.Empty);
	}

	private IEnumerable<DiskSystemEntry> GetChildren(DiskSystemEntry diskSystemEntry)
	{
		if (string.IsNullOrEmpty(diskSystemEntry.Path))
		{
			return from drive in DriveInfo.GetDrives()
				where drive.IsReady
				select DiskSystemEntry.Create(drive.Name);
		}
		return from info in new DirectoryInfo(diskSystemEntry.Path).GetFileSystemInfos()
			where !info.Attributes.HasFlag(FileAttributes.Hidden | FileAttributes.System)
			where info.Attributes.HasFlag(FileAttributes.Directory) || _fileFilter.IsValidFile(info)
			select DiskSystemEntry.Create(info.FullName) into entry
			orderby entry.IsDirectory descending
			select entry;
	}

	private void ShowNoPermissionDialog()
	{
		_dialogBoxShower.Create().SetLocalizedMessage(NoPermissionKey).SetConfirmButton(delegate
		{
			TryOpenDirectory(_currentDirectory.Path);
		})
			.Show();
	}
}
