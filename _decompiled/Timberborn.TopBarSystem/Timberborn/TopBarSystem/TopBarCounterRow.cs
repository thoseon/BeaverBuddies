using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.ResourceCountingSystem;
using Timberborn.ResourceCountingSystemUI;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.TopBarSystem;

public class TopBarCounterRow : ITopBarCounter
{
	private readonly ILoc _loc;

	private readonly ContextualResourceCountingService _contextualResourceCountingService;

	private readonly VisualElement _root;

	private readonly string _goodId;

	private readonly Label _counter;

	private readonly VisualElement _fillGauge;

	private readonly bool _alwaysVisible;

	private int _previousAmount = -1;

	private readonly Phrase _counterPhrase = Phrase.New().FormatCompact();

	public TopBarCounterRow(ILoc loc, ContextualResourceCountingService contextualResourceCountingService, string goodId, VisualElement root, Label counter, VisualElement fillGauge, bool alwaysVisible = false)
	{
		_loc = loc;
		_contextualResourceCountingService = contextualResourceCountingService;
		_goodId = goodId;
		_root = root;
		_counter = counter;
		_fillGauge = fillGauge;
		_alwaysVisible = alwaysVisible;
	}

	public void UpdateValues()
	{
		UpdateAndGetStock(out var _);
	}

	public int UpdateAndGetStock(out bool isVisible)
	{
		ResourceCount contextualResourceCount = _contextualResourceCountingService.GetContextualResourceCount(_goodId);
		_fillGauge.SetHeightAsPercent(contextualResourceCount.FillRate);
		int availableStock = contextualResourceCount.AvailableStock;
		if (_previousAmount != availableStock)
		{
			_counter.text = _loc.T(_counterPhrase, availableStock);
			_previousAmount = availableStock;
		}
		isVisible = _alwaysVisible || contextualResourceCount.AllStock > 0;
		_root.ToggleDisplayStyle(isVisible);
		return availableStock;
	}
}
