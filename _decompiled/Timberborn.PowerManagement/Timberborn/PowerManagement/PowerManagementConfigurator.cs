using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.PowerManagement;

[Context("Game")]
internal class PowerManagementConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<GravityBattery>().AsTransient();
		Bind<Clutch>().AsTransient();
		Bind<ClutchModel>().AsTransient();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<GravityBatterySpec, GravityBattery>();
		builder.AddDecorator<ClutchSpec, Clutch>();
		builder.AddDecorator<ClutchModelSpec, ClutchModel>();
		return builder.Build();
	}
}
