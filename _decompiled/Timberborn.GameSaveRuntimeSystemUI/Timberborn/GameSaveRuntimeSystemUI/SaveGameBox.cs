using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.GameSaveRepositorySystemUI;
using Timberborn.GameSaveRuntimeSystem;
using Timberborn.InputSystem;
using Timberborn.Localization;
using Timberborn.PlatformUtilities;
using Timberborn.SettlementNameSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.GameSaveRuntimeSystemUI;

public class SaveGameBox : IPanelController, ILoadableSingleton
{
	private static readonly string SaveExistsLocKey = "Saving.SaveExists";

	private static readonly string ErrorLocKey = "Saving.Error";

	private readonly GameSaver _gameSaver;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly SaveNameProvider _saveNameProvider;

	private readonly ILoc _loc;

	private readonly GameSaveRepository _gameSaveRepository;

	private readonly SettlementReferenceService _settlementReferenceService;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly IExplorerOpener _explorerOpener;

	private readonly GameSaveItemFactory _gameSaveItemFactory;

	private readonly GameSaveItemElementFactory _gameSaveItemElementFactory;

	private readonly InputService _inputService;

	private VisualElement _root;

	private ListView _saveList;

	private TextField _saveName;

	private Button _saveButton;

	private readonly List<GameSaveItem> _saveItems = new List<GameSaveItem>();

	private bool _isShown;

	private bool SaveNameEntered => !string.IsNullOrWhiteSpace(_saveName.value);

	public SaveGameBox(GameSaver gameSaver, DialogBoxShower dialogBoxShower, SaveNameProvider saveNameProvider, ILoc loc, GameSaveRepository gameSaveRepository, SettlementReferenceService settlementReferenceService, VisualElementLoader visualElementLoader, PanelStack panelStack, IExplorerOpener explorerOpener, GameSaveItemFactory gameSaveItemFactory, GameSaveItemElementFactory gameSaveItemElementFactory, InputService inputService)
	{
		_gameSaver = gameSaver;
		_dialogBoxShower = dialogBoxShower;
		_saveNameProvider = saveNameProvider;
		_loc = loc;
		_gameSaveRepository = gameSaveRepository;
		_settlementReferenceService = settlementReferenceService;
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_explorerOpener = explorerOpener;
		_gameSaveItemFactory = gameSaveItemFactory;
		_gameSaveItemElementFactory = gameSaveItemElementFactory;
		_inputService = inputService;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Options/SaveBox");
		_saveName = _root.Q<TextField>("SaveName");
		_saveName.maxLength = 50;
		_saveName.focusable = true;
		_saveName.RegisterCallback<ChangeEvent<string>>(delegate
		{
			UpdateSaveButton();
		});
		_saveName.Q<TextElement>().SetConfirmCancelActions(_inputService, TrySaveGame, Close);
		_saveList = _root.Q<ListView>("ItemList");
		_saveList.makeItem = CreateAndBind;
		_saveList.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
		_saveList.bindItem = delegate(VisualElement ve, int i)
		{
			_gameSaveItemElementFactory.Bind(ve, _saveItems[i]);
		};
		_saveList.itemsSource = _saveItems;
		_saveList.RegisterCallback<ClickEvent>(delegate
		{
			UpdateSaveName();
		});
		_saveButton = _root.Q<Button>("SaveButton");
		_saveButton.RegisterCallback<ClickEvent>(OnSaveButtonButtonClicked);
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			Close();
		});
		_root.Q<Button>("BrowseDirectoryButton").RegisterCallback<ClickEvent>(OnBrowseDirectoryButtonClicked);
	}

	public void Open()
	{
		_saveItems.AddRange(from save in _gameSaveItemFactory.CreateForSettlement(_settlementReferenceService.SettlementReference)
			where !save.IsAutosave
			select save);
		_saveName.value = _saveNameProvider.GetDefaultSaveName(_saveItems.AsReadOnlyList());
		_panelStack.HideAndPushOverlay(this);
		_saveList.RefreshItems();
		_saveList.ClearSelection();
		_saveList.ScrollToItem(0);
		_saveName.Focus();
		_isShown = true;
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		if (SaveNameEntered)
		{
			TrySaveGame();
			return true;
		}
		return false;
	}

	public void OnUICancelled()
	{
		Close();
	}

	private VisualElement CreateAndBind()
	{
		VisualElement visualElement = _gameSaveItemElementFactory.Create();
		visualElement.RegisterCallback<ClickEvent>(OnSavedGameClick);
		return visualElement;
	}

	private void OnSavedGameClick(ClickEvent evt)
	{
		if (evt.clickCount == 2)
		{
			TrySaveGame();
		}
	}

	private void UpdateSaveButton()
	{
		_saveButton.SetEnabled(SaveNameEntered);
	}

	private void OnSaveButtonButtonClicked(ClickEvent evt)
	{
		TrySaveGame();
	}

	private void UpdateSaveName()
	{
		if (_saveList.selectedItem is GameSaveItem gameSaveItem)
		{
			_saveName.SetValueWithoutNotify(gameSaveItem.DisplayName);
			_saveButton.SetEnabled(value: true);
		}
	}

	private void OnBrowseDirectoryButtonClicked(ClickEvent evt)
	{
		string directory = _gameSaveRepository.SettlementReferenceIntoDirectoryName(_settlementReferenceService.SettlementReference);
		_explorerOpener.OpenDirectory(directory);
	}

	private void TrySaveGame()
	{
		if (SaveNameEntered)
		{
			ValidateAndSave(new SaveReference(_saveName.value, _settlementReferenceService.SettlementReference));
		}
	}

	private void ValidateAndSave(SaveReference saveReference)
	{
		if (_gameSaveRepository.NameIsInvalid(saveReference.SaveName))
		{
			ShowError("Name validation failed for: " + saveReference.SaveName);
		}
		else if (_gameSaveRepository.SaveExists(saveReference))
		{
			ShowOverwriteDialog(saveReference);
		}
		else
		{
			SaveGame(saveReference);
		}
	}

	private void ShowError(string message)
	{
		Debug.LogWarning(message);
		_dialogBoxShower.Create().SetLocalizedMessage(ErrorLocKey).Show();
	}

	private void ShowOverwriteDialog(SaveReference saveReference)
	{
		_dialogBoxShower.Create().SetMessage(_loc.T(SaveExistsLocKey, saveReference.SaveName)).SetConfirmButton(delegate
		{
			SaveGame(saveReference);
		}, _loc.T(CommonLocKeys.OverwriteKey))
			.SetDefaultCancelButton(_loc.T(CommonLocKeys.CancelKey))
			.Show();
	}

	private void SaveGame(SaveReference saveReference)
	{
		try
		{
			_gameSaver.QueueSave(saveReference, Close);
		}
		catch (GameSaverException ex)
		{
			ShowError($"Error occured while saving: {ex.InnerException}");
		}
	}

	private void Close()
	{
		if (_isShown)
		{
			_isShown = false;
			_saveList.ScrollToItem(0);
			_saveItems.Clear();
			_panelStack.Pop(this);
		}
	}
}
