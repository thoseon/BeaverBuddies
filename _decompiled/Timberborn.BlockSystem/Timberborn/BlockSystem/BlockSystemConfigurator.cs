using Bindito.Core;
using Timberborn.TemplateInstantiation;
using Timberborn.TransformControl;

namespace Timberborn.BlockSystem;

[Context("Game")]
[Context("MapEditor")]
internal class BlockSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BlockObject>().AsTransient();
		Bind<BlockObjectAtopDeletionBlocker>().AsTransient();
		Bind<BlockObjectCenter>().AsTransient();
		Bind<BlockObjectRange>().AsTransient();
		Bind<BlockObjectState>().AsTransient();
		Bind<BlockObjectPostLoadState>().AsTransient();
		Bind<BlockObjectTerrainCutout>().AsTransient();
		Bind<BlockOccupant>().AsTransient();
		Bind<PlacementChangeNotifier>().AsTransient();
		Bind<Preview>().AsTransient();
		Bind<PreviewBlockObject>().AsTransient();
		Bind<AreaClamper>().AsSingleton();
		Bind<AreaIterator>().AsSingleton();
		Bind<BlockService>().AsSingleton();
		Bind<IBlockService>().ToExisting<BlockService>();
		Bind<BlockValidator>().AsSingleton();
		Bind<MatterBelowValidator>().AsSingleton();
		Bind<StackableBlockService>().AsSingleton();
		Bind<IBlockOccupancyService>().To<BlockOccupancyService>().AsSingleton();
		Bind<OverridenBlockObjectService>().AsSingleton();
		Bind<BlockObjectFactory>().AsSingleton();
		Bind<PreviewBlockService>().AsSingleton();
		Bind<BlockObjectBatchLoader>().AsSingleton();
		Bind<BlockObjectValidationService>().AsSingleton();
		MultiBind<IBlockObjectValidator>().To<NoTerrainRemoverBelowValidator>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BlockObjectSpec, BlockObject>();
		builder.AddDecorator<BlockObject, TransformController>();
		builder.AddDecorator<BlockObject, PlacementChangeNotifier>();
		builder.AddDecorator<BlockObject, BlockObjectCenter>();
		builder.AddDecorator<BlockObject, BlockObjectRange>();
		builder.AddDecorator<BlockObject, BlockObjectState>();
		builder.AddDecorator<BlockObject, BlockObjectPostLoadState>();
		builder.AddDecorator<BlockObject, Preview>();
		builder.AddDecorator<BlockObject, PreviewBlockObject>();
		builder.AddDecorator<BlockObject, BlockObjectAtopDeletionBlocker>();
		builder.AddDecorator<BlockObjectTerrainCutoutSpec, BlockObjectTerrainCutout>();
		return builder.Build();
	}
}
