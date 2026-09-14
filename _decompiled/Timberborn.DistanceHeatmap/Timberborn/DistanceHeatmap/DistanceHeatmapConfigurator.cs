using Bindito.Core;
using Timberborn.BlockSystemNavigation;
using Timberborn.GameDistricts;
using Timberborn.TemplateInstantiation;

namespace Timberborn.DistanceHeatmap;

[Context("Game")]
internal class DistanceHeatmapConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<DistanceHeatmapShower>().AsTransient();
		Bind<DistanceHeatmapEnabler>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<DistrictCenter, DistanceHeatmapShower>();
		builder.AddDecorator<BlockObjectWithPathRangeSpec, DistanceHeatmapEnabler>();
		return builder.Build();
	}
}
