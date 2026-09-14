using Bindito.Core;
using Timberborn.Beavers;
using Timberborn.EnterableSystem;
using Timberborn.GameDistricts;
using Timberborn.TemplateInstantiation;

namespace Timberborn.DwellingSystem;

[Context("Game")]
internal class DwellingSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<UnreachableHomeUnassigner>().AsTransient();
		Bind<Dwelling>().AsTransient();
		Bind<Dweller>().AsTransient();
		Bind<AutoAssignableDwelling>().AsTransient();
		Bind<DistrictDwellingStatisticsProvider>().AsTransient();
		Bind<DwellerCounter>().AsTransient();
		Bind<DwellerHomeAssigner>().AsSingleton();
		Bind<GlobalDwellingStatisticsProvider>().AsSingleton();
		Bind<StaleAssignableDwellingService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BeaverSpec, Dweller>();
		builder.AddDecorator<Dweller, UnreachableHomeUnassigner>();
		builder.AddDecorator<DwellingSpec, Dwelling>();
		builder.AddDecorator<Dwelling, AutoAssignableDwelling>();
		builder.AddDecorator<Dwelling, DwellerCounter>();
		builder.AddDecorator<Dwelling, EnterableSounds>();
		builder.AddDecorator<DistrictCenter, DistrictDwellingStatisticsProvider>();
		return builder.Build();
	}
}
