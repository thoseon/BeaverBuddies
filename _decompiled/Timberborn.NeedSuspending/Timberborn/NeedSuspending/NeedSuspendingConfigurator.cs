using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.NeedSuspending;

[Context("Game")]
internal class NeedSuspendingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<EntererNeedSuspendingBuilding>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<EntererNeedSuspendingBuildingSpec, EntererNeedSuspendingBuilding>();
		return builder.Build();
	}
}
