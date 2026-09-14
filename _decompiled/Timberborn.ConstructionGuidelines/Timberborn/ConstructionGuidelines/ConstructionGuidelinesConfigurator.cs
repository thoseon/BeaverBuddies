using Bindito.Core;
using Timberborn.BlockSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ConstructionGuidelines;

[Context("Game")]
[Context("MapEditor")]
internal class ConstructionGuidelinesConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BlockObjectGridFootprint>().AsTransient();
		Bind<ConstructionGuidelinesRenderingService>().AsSingleton();
		Bind<TileDrawerFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BlockObject, BlockObjectGridFootprint>();
		return builder.Build();
	}
}
