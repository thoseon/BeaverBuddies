using Bindito.Core;
using Timberborn.Debugging;
using Timberborn.EntityNaming;
using Timberborn.Illumination;
using Timberborn.TemplateInstantiation;

namespace Timberborn.Automation;

[Context("Game")]
internal class AutomationConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<Automator>().AsTransient();
		Bind<Automatable>().AsTransient();
		Bind<AutomatorIlluminator>().AsTransient();
		Bind<AutomationRunner>().AsSingleton();
		Bind<IAutomationRunnerDebugger>().ToExisting<AutomationRunner>();
		Bind<AutomationPartitioner>().AsSingleton();
		Bind<AutomationPlanVersioner>().AsSingleton();
		Bind<AutomatorPartitionFactory>().AsSingleton();
		Bind<AutomatorRegistry>().AsSingleton();
		Bind<AutomationResetter>().AsSingleton();
		Bind<AutomationDebugger>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<IDevModule>().To<AutomationDevModule>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ITransmitter, Automator>();
		builder.AddDecorator<ITerminal, Automator>();
		builder.AddDecorator<IAutomatableNeeder, Automatable>();
		builder.AddDecorator<AutomatorIlluminator, Illuminator>();
		builder.AddDecorator<AutomatorIlluminator, CustomizableIlluminator>();
		builder.AddDecorator<ITransmitter, NumberedEntityNamer>();
		return builder.Build();
	}
}
