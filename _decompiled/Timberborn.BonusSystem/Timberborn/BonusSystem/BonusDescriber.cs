using System;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.BonusSystem;

public class BonusDescriber : ILoadableSingleton
{
	private readonly BonusTypeSpecService _bonusTypeSpecService;

	private readonly ISpecService _specService;

	private Color _positiveBonusHighlight;

	private Color _negativeBonusHighlight;

	public BonusDescriber(BonusTypeSpecService bonusTypeSpecService, ISpecService specService)
	{
		_bonusTypeSpecService = bonusTypeSpecService;
		_specService = specService;
	}

	public void Load()
	{
		BonusDescriberColorsSpec singleSpec = _specService.GetSingleSpec<BonusDescriberColorsSpec>();
		_positiveBonusHighlight = singleSpec.PositiveBonusHighlight;
		_negativeBonusHighlight = singleSpec.NegativeBonusHighlight;
	}

	public string Describe(BonusSpec bonusSpec)
	{
		return Describe(bonusSpec.Id, bonusSpec.MultiplierDelta, colored: false);
	}

	public string DescribeColored(BonusSpec bonusSpec)
	{
		return Describe(bonusSpec.Id, bonusSpec.MultiplierDelta, colored: true);
	}

	public string ColorPositive(string description)
	{
		return Color(description, positive: true);
	}

	public string ColorNegative(string description)
	{
		return Color(description, positive: false);
	}

	private string Describe(string bonusId, float multiplierDelta, bool colored)
	{
		bool flag = multiplierDelta > 0f;
		string text = (flag ? "+" : "-");
		string text2 = $"{Math.Abs(multiplierDelta) * 100f:0}%";
		string text3 = _bonusTypeSpecService.GetSpec(bonusId).DisplayName.Value + ": " + text + text2;
		if (!colored)
		{
			return text3;
		}
		return Color(text3, flag);
	}

	private string Color(string description, bool positive)
	{
		string text = ColorUtility.ToHtmlStringRGB(positive ? _positiveBonusHighlight : _negativeBonusHighlight);
		return "<color=#" + text + ">" + description + "</color>";
	}
}
