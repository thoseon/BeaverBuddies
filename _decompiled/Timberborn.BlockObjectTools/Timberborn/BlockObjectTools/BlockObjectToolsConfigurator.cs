using Bindito.Core;
using Timberborn.TemplateInstantiation;
using Timberborn.TerrainSystem;
using Timberborn.ToolSystem;

namespace Timberborn.BlockObjectTools;

[Context("Game")]
[Context("MapEditor")]
internal class BlockObjectToolsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<PreviewTerrainCutout>().AsTransient();
		Bind<PreviewFactory>().AsSingleton();
		Bind<BlockObjectToolDescriber>().AsSingleton();
		Bind<EntityBlockObjectDeletionTool>().AsSingleton();
		Bind<BlockObjectToolFactory>().AsSingleton();
		Bind<PlaceableBlockObjectSpecService>().AsSingleton();
		Bind<PreviewPlacement>().AsSingleton();
		Bind<BlockObjectPlacerService>().AsSingleton();
		Bind<DefaultBlockObjectPlacer>().AsSingleton();
		Bind<BlockObjectToolGroupSpecService>().AsSingleton();
		Bind<PreviewPlacerFactory>().AsSingleton();
		Bind<PreviewShower>().AsSingleton();
		Bind<PreviewTerrainCutoutService>().AsSingleton();
		MultiBind<IToolFinder>().To<BlockObjectToolFinder>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ICutoutTilesProvider, PreviewTerrainCutout>();
		return builder.Build();
	}
}
