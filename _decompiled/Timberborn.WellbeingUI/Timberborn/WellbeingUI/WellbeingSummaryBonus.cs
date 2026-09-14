using Timberborn.BonusSystem;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class WellbeingSummaryBonus
{
	private static readonly string BonusFormat = "+#%;-#%;0%";

	private readonly BonusManager _bonusManager;

	private readonly Label _bonusValue;

	private readonly string _bonusId;

	public VisualElement Root { get; }

	public WellbeingSummaryBonus(VisualElement root, BonusManager bonusManager, Label bonusValue, string bonusId)
	{
		Root = root;
		_bonusManager = bonusManager;
		_bonusValue = bonusValue;
		_bonusId = bonusId;
	}

	public void UpdateBonus()
	{
		float num = _bonusManager.Multiplier(_bonusId) - 1f;
		_bonusValue.text = num.ToString(BonusFormat);
	}
}
