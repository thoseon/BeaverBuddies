using System;
using System.Collections.Generic;
using System.IO;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.GameSaveRepositorySystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.GameSaveRepositorySystemUI;

public class SettlementList
{
	private readonly GameSaveRepository _gameSaveRepository;

	private readonly VisualElementLoader _visualElementLoader;

	private ListView _settlementListView;

	private readonly List<SettlementReference> _settlements = new List<SettlementReference>();

	private Action _onSettlementSelected;

	private readonly DevModeManager _devModeManager;

	public SettlementList(GameSaveRepository gameSaveRepository, VisualElementLoader visualElementLoader, DevModeManager devModeManager)
	{
		_gameSaveRepository = gameSaveRepository;
		_visualElementLoader = visualElementLoader;
		_devModeManager = devModeManager;
	}

	public void Initialize(VisualElement root)
	{
		Asserts.FieldIsNull(this, _settlementListView, "_settlementListView");
		_settlementListView = root.Q<ListView>("Settlements");
		_settlementListView.makeItem = () => _visualElementLoader.LoadVisualElement("Options/ListViewItem");
		_settlementListView.bindItem = BindSettlement;
		_settlementListView.itemsSource = _settlements;
		_settlementListView.selectionChanged += OnSelectionChanged;
		_settlementListView.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
	}

	public void LoadSettlements(Action onSettlementSelected)
	{
		_onSettlementSelected = onSettlementSelected;
		_settlements.AddRange(_gameSaveRepository.GetAllSettlements());
		_settlementListView.RefreshItems();
		_settlementListView.SetSelection(0);
		_settlementListView.ScrollToItem(0);
	}

	public void Clear()
	{
		_onSettlementSelected = null;
		_settlements.Clear();
		_settlementListView.Clear();
		_settlementListView.ClearSelection();
	}

	public bool TryGetSelectedSettlement(out SettlementReference selectedSettlement)
	{
		selectedSettlement = _settlementListView.selectedItem as SettlementReference;
		return selectedSettlement != null;
	}

	public void DeleteSettlement(SettlementReference settlement)
	{
		_gameSaveRepository.DeleteSettlement(settlement);
		RemoveSettlementFromList(settlement);
	}

	public void RemoveSettlementFromList(SettlementReference settlementName)
	{
		int selectedIndex = _settlementListView.selectedIndex;
		_settlements.Remove(settlementName);
		_settlementListView.RefreshItems();
		SelectSettlementOrLast(selectedIndex);
	}

	private void OnSelectionChanged(IEnumerable<object> obj)
	{
		_onSettlementSelected?.Invoke();
	}

	private void BindSettlement(VisualElement visualElement, int i)
	{
		DevModeManager devModeManager = _devModeManager;
		if (devModeManager != null && devModeManager.Enabled)
		{
			visualElement.Q<Label>("Text").text = Path.GetFileName(_settlements[i].SaveDirectory)[0] + "/" + _settlements[i].SettlementName;
		}
		else
		{
			visualElement.Q<Label>("Text").text = _settlements[i].SettlementName;
		}
	}

	private void SelectSettlementOrLast(int index)
	{
		_settlementListView.ClearSelection();
		int selection = Mathf.Min(index, _settlements.Count - 1);
		_settlementListView.SetSelection(selection);
	}
}
