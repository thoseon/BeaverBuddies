using Bindito.Core;
using Timberborn.BlockSystem;
using Timberborn.CameraSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.StatusSystem;

[Context("Game")]
[Context("MapEditor")]
internal class StatusSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<StatusIconCycler>().AsTransient();
		Bind<StatusSlotOccupier>().AsTransient();
		Bind<StatusSubject>().AsTransient();
		Bind<StatusInstanceFactory>().AsSingleton();
		Bind<StatusIconMaterials>().AsSingleton();
		Bind<StatusSpriteLoader>().AsSingleton();
		Bind<StatusIconCyclerUpdater>().AsSingleton();
		Bind<IStatusIconOffsetService>().To<StatusIconOffsetService>().AsSingleton();
		Bind<StatusIconCyclerFactory>().AsSingleton();
		Bind<StatusAggregator>().AsSingleton();
		Bind<DynamicStatusAggregator>().AsSingleton();
		Bind<StatusSlotUpdateService>().AsSingleton();
		Bind<StatusSlotsUpdater>().AsSingleton();
		Bind<StatusIconSlotFactory>().AsSingleton();
		Bind<StatusIconOffsetCalculator>().AsSingleton();
		Bind<NotifyingStatusMonitor>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<StatusIconCycler, FacingCamera>();
		builder.AddDecorator<BlockObject, StatusSlotOccupier>();
		return builder.Build();
	}
}
