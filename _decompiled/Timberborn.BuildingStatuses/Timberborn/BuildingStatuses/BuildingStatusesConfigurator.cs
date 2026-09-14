using Bindito.Core;
using Timberborn.Buildings;
using Timberborn.StatusSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BuildingStatuses;

[Context("Game")]
[Context("MapEditor")]
internal class BuildingStatusesConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BuildingStatusIconOffsetter>().AsTransient();
		Bind<BuildingStatusIconUpdater>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BuildingSpec, BuildingStatusIconOffsetter>();
		builder.AddDecorator<BuildingSpec, StatusSubject>();
		builder.AddDecorator<BuildingSpec, StatusIconCycler>();
		return builder.Build();
	}
}
