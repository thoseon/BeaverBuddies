using System;
using Timberborn.BatchControl;
using Timberborn.DropdownSystem;
using Timberborn.Gathering;
using UnityEngine.UIElements;

namespace Timberborn.GatheringUI;

internal class GatherablePrioritizerBatchControlRowItem : IBatchControlRowItem, IInitializableBatchControlItem, IClearableBatchControlRowItem
{
	private readonly Dropdown _dropdown;

	private readonly GatherablePrioritizer _gatherablePrioritizer;

	public VisualElement Root { get; }

	public GatherablePrioritizerBatchControlRowItem(VisualElement root, Dropdown dropdown, GatherablePrioritizer gatherablePrioritizer)
	{
		Root = root;
		_dropdown = dropdown;
		_gatherablePrioritizer = gatherablePrioritizer;
	}

	public void InitializeRowItem()
	{
		_gatherablePrioritizer.PrioritizedGatherableChanged += OnPrioritizedGatherableChanged;
	}

	public void ClearRowItem()
	{
		_gatherablePrioritizer.PrioritizedGatherableChanged -= OnPrioritizedGatherableChanged;
	}

	private void OnPrioritizedGatherableChanged(object sender, EventArgs e)
	{
		_dropdown.UpdateSelectedValue();
	}
}
