using System;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.BatchControl;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.DwellingSystem;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.DwellingSystemUI;

public class DwellingBatchControlRowItemFactory
{
	private static readonly string DwellersLocKey = "Dwelling.Dwellers";

	private static readonly string AdultsLocKey = "Beaver.Population.Adults";

	private static readonly string ChildrenLocKey = "Beaver.Population.Children";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	public DwellingBatchControlRowItemFactory(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public IBatchControlRowItem Create(BaseComponent entity)
	{
		Dwelling dwelling = entity.GetComponent<Dwelling>();
		if (dwelling != null)
		{
			string elementName = "Game/BatchControl/DwellingBatchControlRowItem";
			VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
			Label info = visualElement.Q<Label>("Info");
			DwellingBatchControlRowItem result = new DwellingBatchControlRowItem(visualElement, dwelling, info);
			_tooltipRegistrar.Register(visualElement, () => GetTooltipText(dwelling));
			return result;
		}
		return null;
	}

	private string GetTooltipText(Dwelling dwelling)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("<b>" + _loc.T(DwellersLocKey) + "</b>");
		int numberOfAdultDwellers = dwelling.NumberOfAdultDwellers;
		int num = Math.Max(dwelling.AdultSlots, numberOfAdultDwellers);
		stringBuilder.AppendLine($"{_loc.T(AdultsLocKey)}: {numberOfAdultDwellers} / {num}");
		int numberOfChildDwellers = dwelling.NumberOfChildDwellers;
		stringBuilder.AppendLine(string.Format(arg2: dwelling.TotalSlots - num, format: "{0}: {1} / {2}", arg0: _loc.T(ChildrenLocKey), arg1: numberOfChildDwellers));
		return stringBuilder.ToStringWithoutNewLineEnd();
	}
}
