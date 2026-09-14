using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.GameSaveRepositorySystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.GameSaveRepositorySystemUI;

public class SaveList
{
	private readonly GameSaveRepository _gameSaveRepository;

	private readonly SaveThumbnailCache _saveThumbnailCache;

	private readonly GameSaveItemFactory _gameSaveItemFactory;

	private readonly GameSaveItemElementFactory _gameSaveItemElementFactory;

	private readonly List<GameSaveItem> _saves = new List<GameSaveItem>();

	private ListView _saveListView;

	private VisualElement _thumbnail;

	private Image _thumbnailImage;

	private Action _onSaveSelectionChanged;

	private Action _doubleClickAction;

	public int Count => _saves.Count;

	public SaveList(GameSaveRepository gameSaveRepository, SaveThumbnailCache saveThumbnailCache, GameSaveItemFactory gameSaveItemFactory, GameSaveItemElementFactory gameSaveItemElementFactory)
	{
		_gameSaveRepository = gameSaveRepository;
		_saveThumbnailCache = saveThumbnailCache;
		_gameSaveItemFactory = gameSaveItemFactory;
		_gameSaveItemElementFactory = gameSaveItemElementFactory;
	}

	public void Initialize(VisualElement root, Action onSaveSelectionChanged, Action doubleClickAction)
	{
		Asserts.FieldIsNull(this, _saveListView, "_saveListView");
		_saveListView = root.Q<ListView>("Saves");
		_doubleClickAction = doubleClickAction;
		_saveListView.makeItem = CreateAndBind;
		_saveListView.bindItem = delegate(VisualElement ve, int i)
		{
			_gameSaveItemElementFactory.Bind(ve, _saves[i]);
		};
		_saveListView.itemsSource = _saves;
		_saveListView.selectionChanged += OnSaveSelectionChanged;
		_saveListView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
		_thumbnail = root.Q<VisualElement>("Thumbnail");
		_thumbnailImage = root.Q<Image>("ThumbnailImage");
		_onSaveSelectionChanged = onSaveSelectionChanged;
	}

	public void Clear()
	{
		_saves.Clear();
		_saveListView.Clear();
		_saveListView.ClearSelection();
		_saveThumbnailCache.Clear();
	}

	public bool TryGetSelectedSave(out GameSaveItem selectedSave)
	{
		selectedSave = _saveListView.selectedItem as GameSaveItem;
		return selectedSave != null;
	}

	public void DeleteSave(GameSaveItem gameSaveItem)
	{
		int selectedIndex = _saveListView.selectedIndex;
		_gameSaveRepository.DeleteSave(gameSaveItem.SaveReference);
		_saves.Remove(gameSaveItem);
		_saveListView.RefreshItems();
		SelectSaveOrLast(selectedIndex);
	}

	public void UpdateSaves(SettlementReference settlement)
	{
		_saves.Clear();
		if (settlement != null)
		{
			_saves.AddRange(_gameSaveItemFactory.CreateForSettlement(settlement));
		}
		_saveListView.RefreshItems();
		_saveListView.ClearSelection();
		_saveListView.SetSelection(0);
		_saveListView.ScrollToItem(0);
	}

	private VisualElement CreateAndBind()
	{
		VisualElement visualElement = _gameSaveItemElementFactory.Create();
		visualElement.RegisterCallback<ClickEvent>(OnClickEvent);
		return visualElement;
	}

	private void OnClickEvent(ClickEvent evt)
	{
		if (evt.clickCount == 2)
		{
			_doubleClickAction();
		}
	}

	private void OnSaveSelectionChanged(IEnumerable<object> selectedSaves)
	{
		if (TryGetSelectedSave(out var selectedSave))
		{
			_thumbnailImage.image = _saveThumbnailCache.GetThumbnail(selectedSave.SaveReference);
			_thumbnail.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_thumbnailImage.image = null;
			_thumbnail.ToggleDisplayStyle(visible: false);
		}
		_onSaveSelectionChanged?.Invoke();
	}

	private void SelectSaveOrLast(int index)
	{
		_saveListView.ClearSelection();
		int selection = Mathf.Min(index, _saves.Count - 1);
		_saveListView.SetSelection(selection);
	}
}
