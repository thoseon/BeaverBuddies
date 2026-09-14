using Bindito.Core;
using Timberborn.GameDistricts;
using Timberborn.TemplateInstantiation;

namespace Timberborn.PopulationStatisticsSampling;

[Context("Game")]
internal class PopulationStatisticsSamplingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DistrictPopulationSamplesRegistry>().AsTransient();
		Bind<DistrictPopulationBalance>().AsTransient();
		Bind<GlobalPopulationSamplesRegistry>().AsSingleton();
		Bind<PopulationSampler>().AsSingleton();
		Bind<PopulationEventTracker>().AsSingleton();
		Bind<PopulationSamplePackedListSerializer>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DistrictCenter, DistrictPopulationSamplesRegistry>();
		builder.AddDecorator<DistrictCenter, DistrictPopulationBalance>();
		return builder.Build();
	}
}
