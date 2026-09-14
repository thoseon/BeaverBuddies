using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.CoreUI;
using Timberborn.Goods;
using Timberborn.RecoverableGoodSystem;
using UnityEngine.UIElements;

namespace Timberborn.RecoverableGoodSystemUI;

public class RecoverableGoodElement
{
	private readonly Label _label;

	private readonly ImmutableArray<RecoverableGoodItem> _recoverableGoodItems;

	public VisualElement Root { get; }

	public RecoverableGoodElement(VisualElement root, Label label, IEnumerable<RecoverableGoodItem> recoverableGoodItems)
	{
		Root = root;
		_label = label;
		_recoverableGoodItems = recoverableGoodItems.ToImmutableArray();
	}

	public void Update(RecoverableGoodRegistry recoverableGoodRegistry)
	{
		UpdateItems(recoverableGoodRegistry.GoodAmounts);
		_label.ToggleDisplayStyle(recoverableGoodRegistry.TotalAmount > 0);
	}

	private void UpdateItems(IReadOnlyList<GoodAmount> goodAmounts)
	{
		for (int i = 0; i < _recoverableGoodItems.Length; i++)
		{
			int amount = GetAmount(goodAmounts, _recoverableGoodItems[i].GoodId);
			_recoverableGoodItems[i].Update(amount);
		}
	}

	private static int GetAmount(IReadOnlyList<GoodAmount> goodAmounts, string goodId)
	{
		for (int i = 0; i < goodAmounts.Count; i++)
		{
			if (goodAmounts[i].GoodId == goodId)
			{
				return goodAmounts[i].Amount;
			}
		}
		return 0;
	}
}
