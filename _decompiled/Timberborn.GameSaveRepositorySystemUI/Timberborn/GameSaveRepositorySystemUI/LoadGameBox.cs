using System.Linq;
using Timberborn.CoreUI;
using Timberborn.GameSaveRepositorySystem;
using Timberborn.Localization;
using Timberborn.PlatformUtilities;
using Timberborn.SaveMetadataSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.GameSaveRepositorySystemUI;

public class LoadGameBox : IPanelController, ILoadableSingleton
{
	private static readonly string DeleteSettlementPromptLocKey = "Saving.DeleteSettlementPrompt";

	private static readonly string DeleteSavePromptLocKey = "Saving.DeleteSavePrompt";

	private static readonly string ShowSavedModsLocKey = "Modding.ShowSavedMods";

	private readonly GameSaveRepository _gameSaveRepository;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly IExplorerOpener _explorerOpener;

	private readonly ILoc _loc;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PanelStack _panelStack;

	private readonly ValidatingGameLoader _validatingGameLoader;

	private readonly SettlementList _settlementList;

	private readonly SaveList _saveList;

	private readonly GameSaveModBox _gameSaveModBox;

	private readonly GameSaveDeserializer _gameSaveDeserializer;

	private readonly SaveMetadataSerializer _saveMetadataSerializer;

	private VisualElement _root;

	private Button _deleteSettlement;

	private Button _deleteSave;

	private Button _load;

	private Button _showSavedMods;

	public LoadGameBox(GameSaveRepository gameSaveRepository, DialogBoxShower dialogBoxShower, IExplorerOpener explorerOpener, ILoc loc, VisualElementLoader visualElementLoader, PanelStack panelStack, ValidatingGameLoader validatingGameLoader, SettlementList settlementList, SaveList saveList, GameSaveModBox gameSaveModBox, GameSaveDeserializer gameSaveDeserializer, SaveMetadataSerializer saveMetadataSerializer)
	{
		_gameSaveRepository = gameSaveRepository;
		_dialogBoxShower = dialogBoxShower;
		_explorerOpener = explorerOpener;
		_loc = loc;
		_visualElementLoader = visualElementLoader;
		_panelStack = panelStack;
		_validatingGameLoader = validatingGameLoader;
		_settlementList = settlementList;
		_saveList = saveList;
		_gameSaveModBox = gameSaveModBox;
		_gameSaveDeserializer = gameSaveDeserializer;
		_saveMetadataSerializer = saveMetadataSerializer;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Options/LoadGameBox");
		_load = _root.Q<Button>("LoadButton");
		_load.RegisterCallback<ClickEvent>(delegate
		{
			LoadGame();
		});
		_deleteSettlement = _root.Q<Button>("DeleteSettlementButton");
		_deleteSettlement.RegisterCallback<ClickEvent>(OnDeleteSettlementButtonClicked);
		_deleteSave = _root.Q<Button>("DeleteSaveButton");
		_deleteSave.RegisterCallback<ClickEvent>(OnDeleteSaveButtonClicked);
		_showSavedMods = _root.Q<Button>("ShowSavedModsButton");
		_showSavedMods.RegisterCallback<ClickEvent>(OnShowSavedModsButtonClicked);
		_root.Q<Button>("CloseButton").RegisterCallback<ClickEvent>(delegate
		{
			OnUICancelled();
		});
		_root.Q<Button>("BrowseDirectoryButton").RegisterCallback<ClickEvent>(delegate
		{
			_explorerOpener.OpenDirectory(_gameSaveRepository.DefaultSaveDirectory);
		});
		_saveList.Initialize(_root, OnSaveSelectionChanged, OnDoubleClickActionRequested);
		_settlementList.Initialize(_root);
	}

	public void Open()
	{
		_panelStack.HideAndPushOverlay(this);
		_settlementList.LoadSettlements(OnSettlementSelectionChanged);
	}

	public VisualElement GetPanel()
	{
		return _root;
	}

	public bool OnUIConfirmed()
	{
		return LoadGame();
	}

	public void OnUICancelled()
	{
		_settlementList.Clear();
		_saveList.Clear();
		_panelStack.Pop(this);
	}

	private void OnDoubleClickActionRequested()
	{
		LoadGame();
	}

	private bool LoadGame()
	{
		if (_saveList.TryGetSelectedSave(out var selectedSave))
		{
			if (_gameSaveRepository.SaveExists(selectedSave.SaveReference))
			{
				_validatingGameLoader.LoadGame(selectedSave.SaveReference);
				return true;
			}
			Debug.LogWarning("Save: " + selectedSave.DisplayName + " doesn't exist, failed to load.");
		}
		return false;
	}

	private void OnDeleteSettlementButtonClicked(ClickEvent evt)
	{
		if (_settlementList.TryGetSelectedSettlement(out var settlement))
		{
			_dialogBoxShower.Create().SetMessage(_loc.T(DeleteSettlementPromptLocKey, settlement.SettlementName)).SetConfirmButton(delegate
			{
				_settlementList.DeleteSettlement(settlement);
			})
				.SetDefaultCancelButton()
				.Show();
		}
	}

	private void OnDeleteSaveButtonClicked(ClickEvent evt)
	{
		if (_saveList.TryGetSelectedSave(out var gameSaveItem))
		{
			_dialogBoxShower.Create().SetMessage(_loc.T(DeleteSavePromptLocKey, gameSaveItem.DisplayName)).SetConfirmButton(delegate
			{
				DeleteSave(gameSaveItem);
			})
				.SetDefaultCancelButton()
				.Show();
		}
	}

	private void OnShowSavedModsButtonClicked(ClickEvent evt)
	{
		if (_saveList.TryGetSelectedSave(out var selectedSave))
		{
			_gameSaveModBox.Show(selectedSave);
		}
	}

	private void DeleteSave(GameSaveItem gameSaveItem)
	{
		_saveList.DeleteSave(gameSaveItem);
		RemoveSettlementWithoutSavesFromList(gameSaveItem.SaveReference.SettlementReference);
	}

	private void RemoveSettlementWithoutSavesFromList(SettlementReference settlementReference)
	{
		if (_saveList.Count == 0)
		{
			_settlementList.RemoveSettlementFromList(settlementReference);
		}
	}

	private void OnSettlementSelectionChanged()
	{
		if (_settlementList.TryGetSelectedSettlement(out var selectedSettlement))
		{
			_deleteSettlement.SetEnabled(value: true);
			_saveList.UpdateSaves(selectedSettlement);
		}
		else
		{
			_deleteSettlement.SetEnabled(value: false);
			_saveList.UpdateSaves(null);
		}
	}

	private void OnSaveSelectionChanged()
	{
		bool flag = _saveList.TryGetSelectedSave(out var selectedSave);
		_load.SetEnabled(flag);
		_deleteSave.SetEnabled(flag);
		_showSavedMods.ToggleDisplayStyle(visible: false);
		if (flag)
		{
			SaveMetadata saveMetadata = _gameSaveDeserializer.ReadFromSaveFile(selectedSave.SaveReference, _saveMetadataSerializer);
			if (saveMetadata != null && saveMetadata.Mods.Any())
			{
				_showSavedMods.ToggleDisplayStyle(visible: true);
				_showSavedMods.text = _loc.T(ShowSavedModsLocKey, saveMetadata.Mods.Length);
			}
		}
	}
}
