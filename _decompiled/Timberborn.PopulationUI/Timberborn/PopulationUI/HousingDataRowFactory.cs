using System;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.Population;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.PopulationUI;

public class HousingDataRowFactory
{
	private static readonly string OccupiedBedsLocKey = "Dwellings.OccupiedBeds";

	private static readonly string FreeBedsLocKey = "Dwellings.FreeBeds";

	private static readonly string HomelessLocKey = "Beaver.HomelessPlural";

	private readonly ILoc _loc;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	public HousingDataRowFactory(ILoc loc, ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader)
	{
		_loc = loc;
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
	}

	public HousingDataRow Create(VisualElement root, Func<PopulationData> populationDataGetter)
	{
		string elementName = "Game/Population/HousingDataRow";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		root.Add(visualElement);
		_tooltipRegistrar.Register(visualElement, () => GetHousingTooltip(populationDataGetter));
		Label occupiedBedCount = visualElement.Q<Label>("OccupiedBedCount");
		Label freeBedCount = visualElement.Q<Label>("FreeBedCount");
		Label homelessCount = visualElement.Q<Label>("HomelessCount");
		return new HousingDataRow(_loc, occupiedBedCount, freeBedCount, homelessCount, populationDataGetter);
	}

	private string GetHousingTooltip(Func<PopulationData> populationDataGetter)
	{
		BedData bedData = populationDataGetter().BedData;
		return $"{_loc.T(OccupiedBedsLocKey)}: {bedData.OccupiedBeds}" + $"\n{_loc.T(FreeBedsLocKey)}: {bedData.FreeBeds}" + $"\n{_loc.T(HomelessLocKey)}: {bedData.Homeless}";
	}
}
