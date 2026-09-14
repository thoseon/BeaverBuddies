using System;
using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.InventorySystemUI;

public class InformationalRow
{
	private readonly Label _goodAmount;

	private readonly Func<int> _goodAmountGetter;

	private readonly bool _showLimit;

	private readonly Func<int> _limitAmountGetter;

	private readonly Label _limit;

	private readonly Label _separator;

	public string GoodId { get; }

	public VisualElement Root { get; }

	public int CurrentAmount => _goodAmountGetter();

	public InformationalRow(string goodId, VisualElement root, Label goodAmount, Func<int> goodAmountGetter, bool showLimit, Func<int> limitAmountGetter, Label limit, Label separator)
	{
		GoodId = goodId;
		Root = root;
		_goodAmountGetter = goodAmountGetter;
		_goodAmount = goodAmount;
		_showLimit = showLimit;
		_limitAmountGetter = limitAmountGetter;
		_limit = limit;
		_separator = separator;
	}

	public void ShowUpdated()
	{
		_goodAmount.text = CurrentAmount.ToString();
		UpdateLimits();
		Root.ToggleDisplayStyle(visible: true);
	}

	public void Hide()
	{
		Root.ToggleDisplayStyle(visible: false);
	}

	private void UpdateLimits()
	{
		if (_showLimit)
		{
			_limit.text = _limitAmountGetter().ToString();
		}
		_separator.ToggleDisplayStyle(_showLimit);
		_limit.ToggleDisplayStyle(_showLimit);
	}
}
