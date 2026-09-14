using Bindito.Core;
using Timberborn.Debugging;
using Timberborn.EntityPanelSystem;
using Timberborn.ScienceSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.ScienceSystemUI;

[Context("Game")]
[Context("MapEditor")]
internal class ScienceSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly ScienceNeedingBuildingFragment _scienceNeedingBuildingFragment;

		public EntityPanelModuleProvider(ScienceNeedingBuildingFragment scienceNeedingBuildingFragment)
		{
			_scienceNeedingBuildingFragment = scienceNeedingBuildingFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_scienceNeedingBuildingFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<NotEnoughScienceStatus>().AsTransient();
		Bind<ScienceNeedingBuildingDescriber>().AsTransient();
		Bind<UnlockableOnceDescriber>().AsTransient();
		Bind<ScienceCostPerHourFactory>().AsSingleton();
		Bind<ScienceNeedingBuildingFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<IDevModule>().To<ScienceAdder>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<ScienceNeedingBuilding, NotEnoughScienceStatus>();
		builder.AddDecorator<ScienceNeedingBuilding, ScienceNeedingBuildingDescriber>();
		builder.AddDecorator<UnlockableOnceSpec, UnlockableOnceDescriber>();
		return builder.Build();
	}
}
