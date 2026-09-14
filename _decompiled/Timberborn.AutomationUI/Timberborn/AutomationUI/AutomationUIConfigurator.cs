using Bindito.Core;
using Timberborn.Automation;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.AutomationUI;

[Context("Game")]
internal class AutomationUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly TransmitterFragment _transmitterFragment;

		private readonly AutomatableFragment _automatableFragment;

		private readonly SequentialTransmitterResetFragment _sequentialTransmitterResetFragment;

		public EntityPanelModuleProvider(TransmitterFragment transmitterFragment, AutomatableFragment automatableFragment, SequentialTransmitterResetFragment sequentialTransmitterResetFragment)
		{
			_transmitterFragment = transmitterFragment;
			_automatableFragment = automatableFragment;
			_sequentialTransmitterResetFragment = sequentialTransmitterResetFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_sequentialTransmitterResetFragment, 50);
			builder.AddMiddleFragment(_transmitterFragment, 100);
			builder.AddBottomFragment(_automatableFragment, 100);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<AutomationLoopStatus>().AsTransient();
		Bind<SequentialTransmitterDescriber>().AsTransient();
		Bind<TransmitterFragment>().AsSingleton();
		Bind<AutomatableFragment>().AsSingleton();
		Bind<AutomatableBatchControlRowItemFactory>().AsSingleton();
		Bind<AutomationDebuggingPanel>().AsSingleton();
		Bind<TransmitterSelectorInitializer>().AsSingleton();
		Bind<SequentialTransmitterResetFragment>().AsSingleton();
		Bind<AutomationStateIconBuilder>().AsSingleton();
		Bind<TransmitterPickerTool>().AsSingleton();
		Bind<TransmitterPickerToolHighlighter>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Automator, AutomationLoopStatus>();
		builder.AddDecorator<ISequentialTransmitter, SequentialTransmitterDescriber>();
		return builder.Build();
	}
}
