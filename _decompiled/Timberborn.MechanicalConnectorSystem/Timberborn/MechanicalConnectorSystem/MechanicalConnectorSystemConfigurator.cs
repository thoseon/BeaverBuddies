using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.MechanicalConnectorSystem;

[Context("Game")]
internal class MechanicalConnectorSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<MechanicalConnectorActivator>().AsTransient();
		Bind<MechanicalConnectors>().AsTransient();
		Bind<MechanicalConnectorFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<MechanicalConnectorTargetSpec, MechanicalConnectors>();
		builder.AddDecorator<MechanicalConnectors, MechanicalConnectorActivator>();
		return builder.Build();
	}
}
