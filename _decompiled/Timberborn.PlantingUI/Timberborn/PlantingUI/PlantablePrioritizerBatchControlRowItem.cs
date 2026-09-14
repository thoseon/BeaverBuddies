using System;
using Timberborn.BatchControl;
using Timberborn.DropdownSystem;
using Timberborn.Planting;
using UnityEngine.UIElements;

namespace Timberborn.PlantingUI;

internal class PlantablePrioritizerBatchControlRowItem : IBatchControlRowItem, IInitializableBatchControlItem, IClearableBatchControlRowItem
{
	private readonly Dropdown _dropdown;

	private readonly PlantablePrioritizer _plantablePrioritizer;

	public VisualElement Root { get; }

	public PlantablePrioritizerBatchControlRowItem(VisualElement root, Dropdown dropdown, PlantablePrioritizer plantablePrioritizer)
	{
		Root = root;
		_dropdown = dropdown;
		_plantablePrioritizer = plantablePrioritizer;
	}

	public void InitializeRowItem()
	{
		_plantablePrioritizer.PrioritizedPlantableChanged += OnPrioritizedPlantableChanged;
	}

	public void ClearRowItem()
	{
		_plantablePrioritizer.PrioritizedPlantableChanged -= OnPrioritizedPlantableChanged;
	}

	private void OnPrioritizedPlantableChanged(object sender, EventArgs e)
	{
		_dropdown.UpdateSelectedValue();
	}
}
