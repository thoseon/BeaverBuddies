using Bindito.Core;
using Timberborn.TemplateInstantiation;
using Timberborn.Yielding;

namespace Timberborn.YieldingUI;

[Context("Game")]
internal class YieldingUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<YieldRemovingBuildingDescriber>().AsTransient();
		Bind<YieldTooltipFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<YieldRemovingBuilding, YieldRemovingBuildingDescriber>();
		return builder.Build();
	}
}
