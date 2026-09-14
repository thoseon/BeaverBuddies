using System;
using System.IO;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Modding;
using Timberborn.ModdingUI;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuModdingUI;

public class ModManagerBox : IPanelController, ILoadableSingleton, IUpdatableSingleton
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly ModRepository _modRepository;

	private readonly IExplorerOpener _explorerOpener;

	private readonly ModListView _modListView;

	private readonly ModUploaderBox _modUploaderBox;

	private readonly CreateModBox _createModBox;

	private VisualElement _root;

	private Button _uploadButton;

	private Button _downloadButton;

	private Action _downloadAction;

	private bool _isShown;

	public ModManagerBox(VisualElementLoader visualElementLoader, PanelStack panelStack, ModRepository modRepository, IExplorerOpener explorerOpener, ModListView modListView, ModUploaderBox modUploaderBox, CreateModBox createModBox)
	{
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_modRepository = modRepository;
		_explorerOpener = explorerOpener;
		_modListView = modListView;
		_modUploaderBox = modUploaderBox;
		_createModBox = createModBox;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Modding/ModManagerBox");
		Label restartWarning = _root.Q<Label>("RestartWarning");
		restartWarning.ToggleDisplayStyle(visible: false);
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		_root.Q<Button>("ConfirmButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		_root.Q<Button>("BrowseButton").RegisterCallback<ClickEvent>(delegate
		{
			_explorerOpener.OpenDirectory(Path.Combine(UserDataFolder.Folder, UserFolderModsProvider.ModsDirectoryName));
			restartWarning.ToggleDisplayStyle(visible: true);
		});
		_uploadButton = _root.Q<Button>("UploadButton");
		_uploadButton.RegisterCallback<ClickEvent>(delegate
		{
			_modUploaderBox.Show();
		});
		_modListView.Initialize(_root, _modRepository.Mods);
		_modListView.ListChanged += delegate
		{
			restartWarning.ToggleDisplayStyle(visible: true);
		};
		_downloadButton = _root.Q<Button>("DownloadButton");
		_downloadButton.ToggleDisplayStyle(visible: false);
		_downloadButton.RegisterCallback<ClickEvent>(delegate
		{
			_downloadAction();
			restartWarning.ToggleDisplayStyle(visible: true);
		});
		_root.Q<Button>("CreateModButton").RegisterCallback<ClickEvent>(delegate
		{
			_createModBox.Open();
		});
	}

	public void SetDownloadAction(Action action)
	{
		Asserts.FieldIsNotNull(this, action, "action");
		_downloadAction = action;
		_downloadButton.ToggleDisplayStyle(visible: true);
	}

	public void Open()
	{
		_panelStack.HideAndPushOverlay(this);
		_modListView.ResetScroll();
		_uploadButton.ToggleDisplayStyle(_modUploaderBox.HasUploader);
		_isShown = true;
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
		_isShown = false;
		_panelStack.Pop(this);
	}

	public void UpdateSingleton()
	{
		if (_isShown)
		{
			_modListView.Update();
		}
	}
}
