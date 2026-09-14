using System.Linq;
using Timberborn.BatchControl;
using Timberborn.CoreUI;
using Timberborn.EntitySystem;
using Timberborn.PopulationStatisticsSampling;

namespace Timberborn.PopulationStatisticsBatchControl;

internal class PopulationStatisticsRowItemFactory
{
	private readonly PopulationStatisticsGraphFactory _populationStatisticsGraphFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly BatchControlDistrict _batchControlDistrict;

	private readonly GlobalPopulationSamplesRegistry _globalPopulationSamplesRegistry;

	public PopulationStatisticsRowItemFactory(PopulationStatisticsGraphFactory populationStatisticsGraphFactory, VisualElementLoader visualElementLoader, BatchControlDistrict batchControlDistrict, GlobalPopulationSamplesRegistry globalPopulationSamplesRegistry)
	{
		_populationStatisticsGraphFactory = populationStatisticsGraphFactory;
		_visualElementLoader = visualElementLoader;
		_batchControlDistrict = batchControlDistrict;
		_globalPopulationSamplesRegistry = globalPopulationSamplesRegistry;
	}

	public BatchControlRow Create(DistrictPopulationSamplesRegistry districtPopulationSamplesRegistry)
	{
		string elementName = "Game/BatchControl/PopulationStatisticsRowItem";
		return new BatchControlRow(_visualElementLoader.LoadVisualElement(elementName), districtPopulationSamplesRegistry.GetComponent<EntityComponent>(), () => _batchControlDistrict.SelectedDistrict, CreateGraphs(districtPopulationSamplesRegistry.PopulationSampleHistory));
	}

	public BatchControlRow CreateGlobal()
	{
		string elementName = "Game/BatchControl/PopulationStatisticsRowItem";
		return new BatchControlRow(_visualElementLoader.LoadVisualElement(elementName), null, () => !_batchControlDistrict.SelectedDistrict, CreateGraphs(_globalPopulationSamplesRegistry.PopulationSampleHistory));
	}

	private IBatchControlRowItem[] CreateGraphs(PopulationSampleHistory populationSampleHistory)
	{
		return _populationStatisticsGraphFactory.Create(populationSampleHistory).ToArray();
	}
}
