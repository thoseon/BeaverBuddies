using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.MapEditorPlacementRandomizing;

[Context("MapEditor")]
internal class MapEditorPlacementRandomizingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BlockObjectPlacementRandomizer>().AsTransient();
		Bind<BlockObjectPlacementRandomizingService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BlockObjectRandomizablePlacementSpec, BlockObjectPlacementRandomizer>();
		return builder.Build();
	}
}
