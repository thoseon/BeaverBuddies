using Bindito.Core;
using Timberborn.Beavers;
using Timberborn.GameDistricts;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BeaverContaminationSystem;

[Context("Game")]
internal class BeaverContaminationSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ContaminateRootBehavior>().AsTransient();
		Bind<ContaminationApplier>().AsTransient();
		Bind<Contaminable>().AsTransient();
		Bind<ContaminableAnimator>().AsTransient();
		Bind<ContaminationIncubator>().AsTransient();
		Bind<ContaminationNeedEnabler>().AsTransient();
		Bind<DistrictBeaverContaminationStatisticsProvider>().AsTransient();
		Bind<GlobalBeaverContaminationStatisticsProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BeaverSpec, Contaminable>();
		builder.AddDecorator<Contaminable, ContaminableAnimator>();
		builder.AddDecorator<Contaminable, ContaminationNeedEnabler>();
		builder.AddDecorator<Contaminable, ContaminationApplier>();
		builder.AddDecorator<Contaminable, ContaminationIncubator>();
		builder.AddDecorator<DistrictCenter, DistrictBeaverContaminationStatisticsProvider>();
		return builder.Build();
	}
}
