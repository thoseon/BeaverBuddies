using Bindito.Core;
using Timberborn.TemplateInstantiation;
using Timberborn.WaterContaminationBuildings;

namespace Timberborn.WaterContaminationBuildingsUI;

[Context("Game")]
internal class WaterContaminationBuildingsUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<BlockedByContaminationBuildingStatus>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ContaminationBlockableBuilding, BlockedByContaminationBuildingStatus>();
		return builder.Build();
	}
}
