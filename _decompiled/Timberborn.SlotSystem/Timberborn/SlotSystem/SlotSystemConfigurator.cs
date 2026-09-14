using Bindito.Core;
using Timberborn.TemplateInstantiation;

namespace Timberborn.SlotSystem;

[Context("Game")]
internal class SlotSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<FixedSlotManager>().AsTransient();
		Bind<PatrollingSlotInitializer>().AsTransient();
		Bind<PrioritySlotRetriever>().AsTransient();
		Bind<SlotAnimationSynchronizer>().AsTransient();
		Bind<SlotManager>().AsTransient();
		Bind<TransformSlotInitializer>().AsTransient();
		Bind<UnfinishedStateSlotDisabler>().AsTransient();
		Bind<SlotRetriever>().AsSingleton();
		Bind<TransformSlotFactory>().AsSingleton();
		Bind<PatrollingSlotFactory>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<FixedSlotManagerSpec, FixedSlotManager>();
		builder.AddDecorator<FixedSlotManager, SlotManager>();
		builder.AddDecorator<PrioritySlotRetrieverSpec, PrioritySlotRetriever>();
		builder.AddDecorator<SlotAnimationSynchronizerSpec, SlotAnimationSynchronizer>();
		builder.AddDecorator<UnfinishedStateSlotDisablerSpec, UnfinishedStateSlotDisabler>();
		builder.AddDecorator<TransformSlotInitializerSpec, TransformSlotInitializer>();
		builder.AddDecorator<PatrollingSlotInitializerSpec, PatrollingSlotInitializer>();
		return builder.Build();
	}
}
