using Bindito.Core;
using Timberborn.Buildings;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ConstructionMode;

[Context("Game")]
[Context("MapEditor")]
internal class ConstructionModeConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ConstructionModeModel>().AsTransient();
		Bind<ConstructionModeService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BuildingSpec, ConstructionModeModel>();
		return builder.Build();
	}
}
