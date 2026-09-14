using Bindito.Core;
using Timberborn.BuildingRange;
using Timberborn.TemplateInstantiation;

namespace Timberborn.RangedEffectBuildingUI;

[Context("Game")]
internal class RangedEffectBuildingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BuildingWithRangePreviewUpdater>().AsTransient();
		Bind<BuildingWithRangeUpdateService>().AsSingleton();
		Bind<RangeTileMarkerService>().AsSingleton();
		Bind<RangeObjectHighlighterService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<IBuildingWithRange, BuildingWithRangePreviewUpdater>();
		return builder.Build();
	}
}
