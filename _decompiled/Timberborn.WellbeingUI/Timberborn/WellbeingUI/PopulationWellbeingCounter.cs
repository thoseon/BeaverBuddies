using System;
using Timberborn.CoreUI;
using Timberborn.NeedSpecs;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class PopulationWellbeingCounter
{
	private static readonly string PositiveWellbeingClass = "wellbeing--positive";

	private static readonly string NegativeWellbeingClass = "wellbeing--negative";

	private readonly NeedSpec _needSpec;

	private readonly VisualElement _root;

	private readonly VisualElement _bar;

	private readonly VisualElement _barWrapper;

	private readonly Label _appliedCount;

	private readonly Label _averageWellbeingShare;

	public string NeedId { get; }

	public PopulationWellbeingCounter(NeedSpec needSpec, VisualElement root, VisualElement bar, VisualElement barWrapper, Label appliedCount, Label averageWellbeingShare)
	{
		_needSpec = needSpec;
		_root = root;
		_bar = bar;
		_barWrapper = barWrapper;
		_appliedCount = appliedCount;
		_averageWellbeingShare = averageWellbeingShare;
		NeedId = needSpec.Id;
	}

	public void UpdateValues(int appliedNeeds, int totalBeaverCount)
	{
		UpdateVisibility(appliedNeeds);
		UpdateColors();
		UpdateProgress(appliedNeeds, totalBeaverCount);
		UpdateAverageWellbeingShare(appliedNeeds, totalBeaverCount);
	}

	private void UpdateVisibility(int appliedNeeds)
	{
		bool isNeverPositive = _needSpec.IsNeverPositive;
		_root.ToggleDisplayStyle(!isNeverPositive || appliedNeeds > 0);
	}

	private void UpdateColors()
	{
		_bar.EnableInClassList(NegativeWellbeingClass, _needSpec.IsNeverPositive);
		_barWrapper.EnableInClassList(NegativeWellbeingClass, !_needSpec.IsNeverNegative);
	}

	private void UpdateProgress(int appliedNeeds, int totalPopulationCount)
	{
		float value = ((appliedNeeds == 0) ? 0f : ((float)appliedNeeds / (float)totalPopulationCount)) * 100f;
		_bar.style.width = new StyleLength(Length.Percent(value));
		_appliedCount.text = $"{appliedNeeds} / {totalPopulationCount}";
	}

	private void UpdateAverageWellbeingShare(int appliedNeeds, int totalPopulationCount)
	{
		int num = CalculateTotalWellbeing(appliedNeeds, totalPopulationCount);
		float num2 = ((totalPopulationCount == 0) ? 0f : ((float)num / (float)totalPopulationCount));
		_averageWellbeingShare.text = FormatAverageWellbeingShare(num2);
		_averageWellbeingShare.EnableInClassList(PositiveWellbeingClass, num2 > 0f);
		_averageWellbeingShare.EnableInClassList(NegativeWellbeingClass, num2 < 0f);
	}

	private int CalculateTotalWellbeing(int appliedNeeds, int totalPopulationCount)
	{
		int favorableWellbeing = _needSpec.GetFavorableWellbeing();
		int unfavorableWellbeing = _needSpec.GetUnfavorableWellbeing();
		int num = totalPopulationCount - appliedNeeds;
		int num2;
		int num3;
		if (!_needSpec.IsNeverPositive && !_needSpec.IsNeverNegative)
		{
			num2 = appliedNeeds * favorableWellbeing;
			num3 = num * unfavorableWellbeing;
		}
		else
		{
			num2 = (_needSpec.IsNeverNegative ? (appliedNeeds * favorableWellbeing) : 0);
			num3 = (_needSpec.IsNeverPositive ? (appliedNeeds * unfavorableWellbeing) : 0);
		}
		return num2 + num3;
	}

	private static string FormatAverageWellbeingShare(float averageWellbeingShare)
	{
		string arg = ((averageWellbeingShare == 0f) ? string.Empty : ((averageWellbeingShare < 0f) ? "-" : "+"));
		return $"{arg}{Math.Abs(averageWellbeingShare):0.0}";
	}
}
