using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.GameDistrictsMigration;
using Timberborn.Localization;
using Timberborn.Population;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class ManualMigrationPopulationRowFactory
{
	private static readonly string AdultIcon = "population-counter__icon--adult";

	private static readonly string ChildIcon = "population-counter__icon--child";

	private static readonly string BotIcon = "population-counter__icon--bot";

	private static readonly string ContaminatedIcon = "population-counter__icon--contamination";

	private static readonly string MinimumLocKey = "Migration.Minimum";

	private readonly ILoc _loc;

	private readonly ManualMigrationBlocker _manualMigrationBlocker;

	private readonly PopulationService _populationService;

	private readonly PopulationDistributorRetriever _populationDistributorRetriever;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly VisualElementLoader _visualElementLoader;

	public ManualMigrationPopulationRowFactory(ILoc loc, ManualMigrationBlocker manualMigrationBlocker, PopulationService populationService, PopulationDistributorRetriever populationDistributorRetriever, ITooltipRegistrar tooltipRegistrar, VisualElementLoader visualElementLoader)
	{
		_loc = loc;
		_manualMigrationBlocker = manualMigrationBlocker;
		_populationService = populationService;
		_populationDistributorRetriever = populationDistributorRetriever;
		_tooltipRegistrar = tooltipRegistrar;
		_visualElementLoader = visualElementLoader;
	}

	public List<ManualMigrationPopulationRow> CreateLeftRows()
	{
		return CreatePopulationRows("Game/BatchControl/ManualMigrationPopulationRowLeft");
	}

	public List<ManualMigrationPopulationRow> CreateRightRows()
	{
		return CreatePopulationRows("Game/BatchControl/ManualMigrationPopulationRowRight");
	}

	private List<ManualMigrationPopulationRow> CreatePopulationRows(string rowTemplate)
	{
		return new List<ManualMigrationPopulationRow>
		{
			CreatePopulationRow<AdultsDistributorTemplate>(rowTemplate, AdultIcon, () => true),
			CreatePopulationRow<ChildrenDistributorTemplate>(rowTemplate, ChildIcon, () => true),
			CreatePopulationRow<ContaminatedDistributorTemplate>(rowTemplate, ContaminatedIcon, () => _populationService.IsAnyoneContaminated),
			CreatePopulationRow<BotsDistributorTemplate>(rowTemplate, BotIcon, () => _populationService.BotCreated)
		};
	}

	private ManualMigrationPopulationRow CreatePopulationRow<T>(string rowTemplate, string icon, Func<bool> visibilityGetter) where T : BaseComponent, IDistributorTemplate
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/BatchControl/BatchControlRow");
		VisualElement visualElement2 = _visualElementLoader.LoadVisualElement(rowTemplate);
		visualElement.Add(visualElement2);
		visualElement2.Q<VisualElement>("PopulationIcon").AddToClassList(icon);
		visualElement2.Q<Label>("MinimumLabel").text = _loc.T(MinimumLocKey) + ":";
		ManualMigrationPopulationRow manualMigrationPopulationRow = new ManualMigrationPopulationRow(_manualMigrationBlocker, _tooltipRegistrar, visualElement, _populationDistributorRetriever.GetPopulationDistributor<T>, visibilityGetter);
		manualMigrationPopulationRow.Initialize();
		return manualMigrationPopulationRow;
	}
}
