using Bindito.Core;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.SelectionSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BlockSystemUI;

[Context("Game")]
[Context("MapEditor")]
internal class BlockSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BlockObjectCameraTarget>().AsTransient();
		Bind<BlockObjectDeletionDescriber>().AsTransient();
		Bind<EntranceMarkerDrawer>().AsTransient();
		Bind<PlaceableBlockObjectDescriber>().AsTransient();
		Bind<UndergroundDepthDescriber>().AsTransient();
		Bind<BlockObjectBoundsDrawerFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BlockObject, EntranceMarkerDrawer>();
		builder.AddDecorator<BlockObject, SelectableObject>();
		builder.AddDecorator<BlockObject, BlockObjectDeletionDescriber>();
		builder.AddDecorator<BlockObjectCenter, BlockObjectCameraTarget>();
		builder.AddDecorator<PlaceableBlockObjectSpec, LabeledEntityBadge>();
		builder.AddDecorator<PlaceableBlockObjectSpec, PlaceableBlockObjectDescriber>();
		builder.AddDecorator<UndergroundDepthDescriberSpec, UndergroundDepthDescriber>();
		builder.AddDecorator<IInfiniteUndergroundModel, UndergroundDepthDescriber>();
		return builder.Build();
	}
}
