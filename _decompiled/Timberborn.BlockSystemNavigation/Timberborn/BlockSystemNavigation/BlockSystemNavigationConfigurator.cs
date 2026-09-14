using Bindito.Core;
using Timberborn.BlockSystem;
using Timberborn.Navigation;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BlockSystemNavigation;

[Context("Game")]
internal class BlockSystemNavigationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BlockObjectNavMesh>().AsTransient();
		Bind<BlockObjectNavMeshAdder>().AsTransient();
		Bind<BlockObjectPreviewNavMesh>().AsTransient();
		Bind<NavMeshObjectUpdater>().AsSingleton();
		Bind<BlockObjectNavMeshGroupInitializer>().AsSingleton();
		Bind<INavMeshSizeProvider>().To<BlockSystemNavMeshSizeProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BlockObject, BlockObjectNavMesh>();
		builder.AddDecorator<Preview, BlockObjectPreviewNavMesh>();
		builder.AddDecorator<BlockObjectNavMeshAdderSpec, BlockObjectNavMeshAdder>();
		return builder.Build();
	}
}
