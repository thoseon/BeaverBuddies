using Bindito.Core;
using Timberborn.TemplateInstantiation;
using Timberborn.WaterBuildings;

namespace Timberborn.WaterContaminationBuildings;

[Context("Game")]
internal class WaterContaminationBuildingsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ContaminationBlockableBuilding>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<IWaterNeedingBuilding, ContaminationBlockableBuilding>();
		return builder.Build();
	}
}
