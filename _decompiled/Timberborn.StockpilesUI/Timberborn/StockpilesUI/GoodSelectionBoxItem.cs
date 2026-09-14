using Timberborn.CoreUI;
using Timberborn.ResourceCountingSystem;
using Timberborn.ResourceCountingSystemUI;
using UnityEngine.UIElements;

namespace Timberborn.StockpilesUI;

internal class GoodSelectionBoxItem
{
	private static readonly string SelectedItemClass = "selected-item";

	private readonly ContextualResourceCountingService _contextualResourceCountingService;

	private readonly string _goodId;

	private readonly VisualElement _counter;

	public VisualElement Root { get; }

	public GoodSelectionBoxItem(ContextualResourceCountingService contextualResourceCountingService, VisualElement root, string goodId, VisualElement counter)
	{
		_contextualResourceCountingService = contextualResourceCountingService;
		Root = root;
		_goodId = goodId;
		_counter = counter;
	}

	public void Update()
	{
		ResourceCount contextualResourceCount = _contextualResourceCountingService.GetContextualResourceCount(_goodId);
		_counter.SetHeightAsPercent(contextualResourceCount.FillRate);
		_counter.parent.ToggleDisplayStyle(contextualResourceCount.AvailableStock > 0);
	}

	public void UpdateSelectedState(string selectedGoodId)
	{
		Root.EnableInClassList(SelectedItemClass, selectedGoodId == _goodId);
	}
}
