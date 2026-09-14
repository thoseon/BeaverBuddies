using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BonusSystem;
using Timberborn.CoreUI;
using Timberborn.TooltipSystem;
using Timberborn.Wellbeing;
using UnityEngine.UIElements;

namespace Timberborn.WellbeingUI;

public class WellbeingSummaryFactory
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly WellbeingSummaryBonusFactory _wellbeingSummaryBonusFactory;

	private readonly WellbeingNameHelper _wellbeingNameHelper;

	public WellbeingSummaryFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, WellbeingSummaryBonusFactory wellbeingSummaryBonusFactory, WellbeingNameHelper wellbeingNameHelper)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_wellbeingSummaryBonusFactory = wellbeingSummaryBonusFactory;
		_wellbeingNameHelper = wellbeingNameHelper;
	}

	public WellbeingSummary Create(BaseComponent entity)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/WellbeingSummaryFragment");
		WellbeingBonusesSubjectSpec component = entity.GetComponent<WellbeingBonusesSubjectSpec>();
		WellbeingTracker component2 = entity.GetComponent<WellbeingTracker>();
		IEnumerable<WellbeingSummaryBonus> wellbeingSummaryBonuses = CreateBonuses(visualElement, component, entity);
		Label wellbeingValue = visualElement.Q<Label>("WellbeingText");
		CreateWellbeingTooltip(visualElement, component2);
		WellbeingSummary wellbeingSummary = new WellbeingSummary(visualElement, component2, wellbeingValue, wellbeingSummaryBonuses);
		wellbeingSummary.UpdateContent();
		return wellbeingSummary;
	}

	private IEnumerable<WellbeingSummaryBonus> CreateBonuses(VisualElement root, WellbeingBonusesSubjectSpec wellbeingBonusesSubjectSpec, BaseComponent entity)
	{
		List<WellbeingSummaryBonus> list = new List<WellbeingSummaryBonus>();
		BonusManager component = entity.GetComponent<BonusManager>();
		foreach (string bonuse in wellbeingBonusesSubjectSpec.Bonuses)
		{
			WellbeingSummaryBonus wellbeingSummaryBonus = _wellbeingSummaryBonusFactory.Create(component, bonuse);
			root.Add(wellbeingSummaryBonus.Root);
			list.Add(wellbeingSummaryBonus);
		}
		return list;
	}

	private void CreateWellbeingTooltip(VisualElement root, WellbeingTracker wellbeingTracker)
	{
		VisualElement visualElement = root.Q<VisualElement>("Wellbeing");
		_tooltipRegistrar.RegisterUpdatable(visualElement, () => GetWellbeingTooltipText(wellbeingTracker));
	}

	private string GetWellbeingTooltipText(WellbeingTracker wellbeingTracker)
	{
		string wellbeingName = _wellbeingNameHelper.GetWellbeingName(wellbeingTracker);
		return $"{wellbeingName}: {wellbeingTracker.Wellbeing}";
	}
}
