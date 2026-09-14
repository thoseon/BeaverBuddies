using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.GameDistricts;
using Timberborn.GameDistrictsMigration;
using Timberborn.Population;
using UnityEngine.UIElements;

namespace Timberborn.GameDistrictsMigrationBatchControl;

internal class MigrationBatchControlRowFactory
{
	private static readonly string AdultIcon = "population-counter__icon--adult";

	private static readonly string ChildIcon = "population-counter__icon--child";

	private static readonly string BotIcon = "population-counter__icon--bot";

	private static readonly string ContaminatedIcon = "population-counter__icon--contamination";

	private static readonly string MarginLeftClass = "migration-batch-control-row__margin-left";

	private readonly CurrentPopulationBatchControlRowItemFactory _currentPopulationBatchControlRowItemFactory;

	private readonly PopulationDataBatchControlRowItemFactory _populationDataBatchControlRowItemFactory;

	private readonly PopulationDistributorBatchControlRowItemFactory _populationDistributorBatchControlRowItemFactory;

	private readonly PopulationDistributorRetriever _populationDistributorRetriever;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly PopulationService _populationService;

	public MigrationBatchControlRowFactory(CurrentPopulationBatchControlRowItemFactory currentPopulationBatchControlRowItemFactory, PopulationDataBatchControlRowItemFactory populationDataBatchControlRowItemFactory, PopulationDistributorBatchControlRowItemFactory populationDistributorBatchControlRowItemFactory, PopulationDistributorRetriever populationDistributorRetriever, VisualElementLoader visualElementLoader, PopulationService populationService)
	{
		_currentPopulationBatchControlRowItemFactory = currentPopulationBatchControlRowItemFactory;
		_populationDataBatchControlRowItemFactory = populationDataBatchControlRowItemFactory;
		_populationDistributorBatchControlRowItemFactory = populationDistributorBatchControlRowItemFactory;
		_populationDistributorRetriever = populationDistributorRetriever;
		_visualElementLoader = visualElementLoader;
		_populationService = populationService;
	}

	public BatchControlRow CreateAdultRow(DistrictCenter districtCenter)
	{
		VisualElement root = CreateRoot();
		PopulationDistributor populationDistributor = _populationDistributorRetriever.GetPopulationDistributor<AdultsDistributorTemplate>(districtCenter);
		return new BatchControlRow(root, populationDistributor.DistrictCenter.GetComponent<EntityComponent>(), _currentPopulationBatchControlRowItemFactory.Create(populationDistributor, AdultIcon), _populationDistributorBatchControlRowItemFactory.Create(populationDistributor), _populationDataBatchControlRowItemFactory.CreateBeaverWorkplaceRowItem(populationDistributor.DistrictCenter));
	}

	public BatchControlRow CreateChildRow(DistrictCenter districtCenter)
	{
		VisualElement root = CreateRoot();
		PopulationDistributor populationDistributor = _populationDistributorRetriever.GetPopulationDistributor<ChildrenDistributorTemplate>(districtCenter);
		return new BatchControlRow(root, populationDistributor.DistrictCenter.GetComponent<EntityComponent>(), _currentPopulationBatchControlRowItemFactory.Create(populationDistributor, ChildIcon), _populationDistributorBatchControlRowItemFactory.Create(populationDistributor));
	}

	public BatchControlRow CreateBotRow(DistrictCenter districtCenter)
	{
		VisualElement root = CreateRoot();
		PopulationDistributor populationDistributor = _populationDistributorRetriever.GetPopulationDistributor<BotsDistributorTemplate>(districtCenter);
		return new BatchControlRow(root, populationDistributor.DistrictCenter.GetComponent<EntityComponent>(), () => _populationService.BotCreated, _currentPopulationBatchControlRowItemFactory.Create(populationDistributor, BotIcon), _populationDistributorBatchControlRowItemFactory.Create(populationDistributor), _populationDataBatchControlRowItemFactory.CreateBotWorkplaceRowItem(populationDistributor.DistrictCenter));
	}

	public BatchControlRow CreateContaminatedRow(DistrictCenter districtCenter)
	{
		VisualElement root = CreateRoot();
		PopulationDistributor populationDistributor = _populationDistributorRetriever.GetPopulationDistributor<ContaminatedDistributorTemplate>(districtCenter);
		return new BatchControlRow(root, populationDistributor.DistrictCenter.GetComponent<EntityComponent>(), () => _populationService.IsAnyoneContaminated, _currentPopulationBatchControlRowItemFactory.Create(populationDistributor, ContaminatedIcon), _populationDistributorBatchControlRowItemFactory.Create(populationDistributor));
	}

	private VisualElement CreateRoot()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Game/BatchControl/BatchControlRow");
		visualElement.AddToClassList(MarginLeftClass);
		return visualElement;
	}
}
