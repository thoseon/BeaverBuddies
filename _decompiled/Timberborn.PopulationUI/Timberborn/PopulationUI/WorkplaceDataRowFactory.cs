using System;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.Population;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.PopulationUI;

public class WorkplaceDataRowFactory
{
	private static readonly string OccupiedWorkslotsLocKey = "Work.WorkersLabel";

	private static readonly string FreeWorkslotsLocKey = "Work.VacantPlural";

	private static readonly string UnemployedLocKey = "Beaver.UnemployedPlural";

	private static readonly string UnavailableLocKey = "Work.Incapacitated";

	private static readonly string BeaversLocKey = "Beaver.PluralDisplayName";

	private static readonly string BotsLocKey = "Bot.PluralDisplayName";

	private readonly ILoc _loc;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	public WorkplaceDataRowFactory(ILoc loc, ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader)
	{
		_loc = loc;
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
	}

	public WorkplaceDataRow CreateBeaverWorkplaceDataRow(VisualElement root, Func<PopulationData> populationDataGetter)
	{
		return Create(root, "Game/Population/BeaverWorkplaceDataRow", () => populationDataGetter().BeaverWorkplaceData, () => populationDataGetter().BeaverWorkforceData, BeaversLocKey);
	}

	public WorkplaceDataRow CreateBotWorkplaceDataRow(VisualElement root, Func<PopulationData> populationDataGetter)
	{
		return Create(root, "Game/Population/BotWorkplaceDataRow", () => populationDataGetter().BotWorkplaceData, () => populationDataGetter().BotWorkforceData, BotsLocKey);
	}

	private WorkplaceDataRow Create(VisualElement root, string elementName, Func<WorkplaceData> workplaceDataGetter, Func<WorkforceData> workforceDataGetter, string headerLocKey)
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		root.Add(visualElement);
		_tooltipRegistrar.Register(visualElement, () => GetWorkplaceData(workplaceDataGetter(), workforceDataGetter(), headerLocKey));
		return new WorkplaceDataRow(_loc, root.Q<Label>("OccupiedWorkslotCount"), root.Q<Label>("FreeWorkslotCount"), root.Q<Label>("UnemployedCount"), workplaceDataGetter);
	}

	private string GetWorkplaceData(WorkplaceData workplaceData, WorkforceData workforceData, string headerLocKey)
	{
		int unemployable = workforceData.Unemployable;
		string text = ((unemployable > 0) ? $"\n{_loc.T(UnavailableLocKey)}: {unemployable}" : "");
		return "<b>" + _loc.T(headerLocKey) + "</b>" + $"\n{_loc.T(OccupiedWorkslotsLocKey)}: {workplaceData.OccupiedWorkslots}" + $"\n{_loc.T(FreeWorkslotsLocKey)}: {workplaceData.FreeWorkslots}" + $"\n{_loc.T(UnemployedLocKey)}: {workplaceData.Unemployed}" + text;
	}
}
