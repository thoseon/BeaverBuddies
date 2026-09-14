using System;
using Timberborn.BatchControl;
using Timberborn.DropdownSystem;
using Timberborn.Workshops;
using UnityEngine.UIElements;

namespace Timberborn.WorkshopsUI;

internal class ManufactoryBatchControlRowItem : IBatchControlRowItem, IInitializableBatchControlItem, IClearableBatchControlRowItem
{
	private readonly Dropdown _dropdown;

	private readonly Manufactory _manufactory;

	public VisualElement Root { get; }

	public ManufactoryBatchControlRowItem(VisualElement root, Dropdown dropdown, Manufactory manufactory)
	{
		Root = root;
		_dropdown = dropdown;
		_manufactory = manufactory;
	}

	public void InitializeRowItem()
	{
		_manufactory.RecipeChanged += OnProductionRecipeChanged;
	}

	public void ClearRowItem()
	{
		_manufactory.RecipeChanged -= OnProductionRecipeChanged;
	}

	private void OnProductionRecipeChanged(object sender, EventArgs e)
	{
		_dropdown.UpdateSelectedValue();
	}
}
