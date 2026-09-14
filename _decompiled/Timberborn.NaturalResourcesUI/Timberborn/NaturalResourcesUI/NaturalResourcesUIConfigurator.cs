using Bindito.Core;
using Timberborn.Debugging;
using Timberborn.NaturalResources;
using Timberborn.NaturalResourcesModelSystem;
using Timberborn.Rendering;
using Timberborn.TemplateInstantiation;

namespace Timberborn.NaturalResourcesUI;

[Context("Game")]
[Context("MapEditor")]
internal class NaturalResourcesUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<NaturalResourceDescriber>().AsTransient();
		Bind<NaturalResourceEntityBadge>().AsTransient();
		Bind<NaturalResourceMarkerPositionUpdater>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<IDevModule>().To<NaturalResourcesModelToggler>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<NaturalResourceSpec, NaturalResourceEntityBadge>();
		builder.AddDecorator<NaturalResourceSpec, NaturalResourceDescriber>();
		builder.AddDecorator<NaturalResourceModel, MarkerPosition>();
		builder.AddDecorator<NaturalResourceModel, NaturalResourceMarkerPositionUpdater>();
		return builder.Build();
	}
}
