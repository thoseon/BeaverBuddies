using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ScienceSystem;

[Context("Game")]
[Context("MapEditor")]
internal class ScienceSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ScienceNeedingBuilding>().AsTransient();
		Bind<BuildingUnlockingService>().AsSingleton();
		Bind<ScienceService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ScienceNeedingBuildingSpec, ScienceNeedingBuilding>();
		return builder.Build();
	}
}
