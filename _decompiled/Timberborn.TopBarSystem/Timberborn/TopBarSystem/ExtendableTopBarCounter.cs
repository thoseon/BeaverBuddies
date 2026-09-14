using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.TopBarSystem;

internal class ExtendableTopBarCounter : ITopBarCounter
{
	private readonly ILoc _loc;

	private readonly ImmutableArray<TopBarCounterRow> _counterRows;

	private readonly Label _emptyCounterPlaceholder;

	private readonly Label _value;

	private int _previousSum = -1;

	private readonly Phrase _numberPhrase = Phrase.New().FormatCompact();

	public ExtendableTopBarCounter(ILoc loc, IEnumerable<TopBarCounterRow> counterRows, Label emptyCounterPlaceholder, Label value)
	{
		_loc = loc;
		_counterRows = counterRows.ToImmutableArray();
		_emptyCounterPlaceholder = emptyCounterPlaceholder;
		_value = value;
	}

	public void UpdateValues()
	{
		int stockSum = GetStockSum(out var anyRowVisible);
		if (_previousSum != stockSum)
		{
			_value.text = _loc.T(_numberPhrase, stockSum);
			_previousSum = stockSum;
		}
		_emptyCounterPlaceholder.ToggleDisplayStyle(!anyRowVisible);
	}

	private int GetStockSum(out bool anyRowVisible)
	{
		int num = 0;
		anyRowVisible = false;
		foreach (TopBarCounterRow counterRow in _counterRows)
		{
			num += counterRow.UpdateAndGetStock(out var isVisible);
			anyRowVisible |= isVisible;
		}
		return num;
	}
}
