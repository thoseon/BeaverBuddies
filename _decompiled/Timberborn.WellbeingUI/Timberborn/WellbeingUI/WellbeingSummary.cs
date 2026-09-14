using System.Collections.Generic;
using Timberborn.Wellbeing;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class WellbeingSummary
{
	private static readonly string NegativeWellbeingClass = "wellbeing--negative";

	private readonly WellbeingTracker _wellbeingTracker;

	private readonly Label _wellbeingValue;

	private readonly IEnumerable<WellbeingSummaryBonus> _wellbeingSummaryBonuses;

	public VisualElement Root { get; }

	public WellbeingSummary(VisualElement root, WellbeingTracker wellbeingTracker, Label wellbeingValue, IEnumerable<WellbeingSummaryBonus> wellbeingSummaryBonuses)
	{
		Root = root;
		_wellbeingTracker = wellbeingTracker;
		_wellbeingValue = wellbeingValue;
		_wellbeingSummaryBonuses = wellbeingSummaryBonuses;
	}

	public void UpdateContent()
	{
		UpdateWellbeing();
		UpdateBonuses();
	}

	private void UpdateWellbeing()
	{
		int wellbeing = _wellbeingTracker.Wellbeing;
		_wellbeingValue.text = wellbeing.ToString();
		_wellbeingValue.EnableInClassList(NegativeWellbeingClass, wellbeing < 0);
	}

	private void UpdateBonuses()
	{
		foreach (WellbeingSummaryBonus wellbeingSummaryBonuse in _wellbeingSummaryBonuses)
		{
			wellbeingSummaryBonuse.UpdateBonus();
		}
	}
}
