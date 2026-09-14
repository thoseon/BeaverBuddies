using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.Illumination;
using Timberborn.TemplateInstantiation;
using Timberborn.WellbeingUI;
using Timberborn.WorkSystem;

namespace Timberborn.WorkSystemUI;

[Context("Game")]
internal class WorkSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly WorkplaceFragment _workplaceFragment;

		public EntityPanelModuleProvider(WorkplaceFragment workplaceFragment)
		{
			_workplaceFragment = workplaceFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddTopFragment(_workplaceFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<WorkplaceIlluminator>().AsTransient();
		Bind<NoUnemployedStatus>().AsTransient();
		Bind<WorkerTypeIlluminator>().AsTransient();
		Bind<WorkplaceBonusesDescriber>().AsTransient();
		Bind<WorkplaceDescriber>().AsTransient();
		Bind<WorkingHoursPanel>().AsSingleton();
		Bind<WorkerViewFactory>().AsSingleton();
		Bind<WorkplaceFragment>().AsSingleton();
		Bind<WorkplaceBatchControlRowItemFactory>().AsSingleton();
		Bind<WorkplacePrioritySpriteLoader>().AsSingleton();
		Bind<WorkplacePriorityToggleGroupFactory>().AsSingleton();
		Bind<WorkplacePriorityBatchControlRowItemFactory>().AsSingleton();
		Bind<WorkplaceWorkerTypeBatchControlRowItemFactory>().AsSingleton();
		Bind<WorkerTypeToggleFactory>().AsSingleton();
		Bind<WorkplaceUnlockingDialogService>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<INeedEffectDescriber>().To<WorkSystemNeedEffectDescriber>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Workplace, WorkplaceDescriber>();
		builder.AddDecorator<Workplace, NoUnemployedStatus>();
		builder.AddDecorator<WorkplaceBonuses, WorkplaceBonusesDescriber>();
		builder.AddDecorator<WorkplaceIlluminatorSpec, WorkplaceIlluminator>();
		builder.AddDecorator<WorkplaceIlluminator, WorkerTypeIlluminator>();
		builder.AddDecorator<WorkplaceIlluminator, Illuminator>();
		return builder.Build();
	}
}
