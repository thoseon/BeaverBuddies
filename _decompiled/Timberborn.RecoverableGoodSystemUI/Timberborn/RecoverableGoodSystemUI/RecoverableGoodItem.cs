using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.RecoverableGoodSystemUI;

public class RecoverableGoodItem
{
	private readonly Label _amountLabel;

	public VisualElement Root { get; }

	public string GoodId { get; }

	public RecoverableGoodItem(VisualElement root, string goodId, Label amountLabel)
	{
		Root = root;
		GoodId = goodId;
		_amountLabel = amountLabel;
	}

	public void Update(int amount)
	{
		Root.ToggleDisplayStyle(amount > 0);
		if (amount > 0)
		{
			_amountLabel.text = amount.ToString();
		}
	}
}
