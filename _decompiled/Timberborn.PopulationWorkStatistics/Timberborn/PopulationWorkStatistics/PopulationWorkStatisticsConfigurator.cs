using Bindito.Core;
using Timberborn.GameDistricts;
using Timberborn.TemplateInstantiation;
using Timberborn.WorkSystem;

namespace Timberborn.PopulationWorkStatistics;

[Context("Game")]
internal class PopulationWorkStatisticsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DistrictEmploymentStatisticsProvider>().AsTransient();
		Bind<DistrictWorkRefusingStatisticsProvider>().AsTransient();
		Bind<WorkplaceWorkerCounter>().AsTransient();
		Bind<GlobalEmploymentStatisticsProvider>().AsSingleton();
		Bind<GlobalWorkRefusingStatisticsProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DistrictCenter, DistrictEmploymentStatisticsProvider>();
		builder.AddDecorator<DistrictCenter, DistrictWorkRefusingStatisticsProvider>();
		builder.AddDecorator<Workplace, WorkplaceWorkerCounter>();
		return builder.Build();
	}
}
