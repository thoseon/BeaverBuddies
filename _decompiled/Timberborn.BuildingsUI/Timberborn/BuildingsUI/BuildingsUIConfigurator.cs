using Bindito.Core;
using Timberborn.AreaSelectionSystem;
using Timberborn.Buildings;
using Timberborn.Debugging;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;

namespace Timberborn.BuildingsUI;

[Context("Game")]
internal class BuildingsUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly DeleteBuildingFragment _deleteBuildingFragment;

		private readonly PausableBuildingFragment _pausableBuildingFragment;

		private readonly BuildingSoundControllerFragment _buildingSoundControllerFragment;

		public EntityPanelModuleProvider(DeleteBuildingFragment deleteBuildingFragment, PausableBuildingFragment pausableBuildingFragment, BuildingSoundControllerFragment buildingSoundControllerFragment)
		{
			_deleteBuildingFragment = deleteBuildingFragment;
			_pausableBuildingFragment = pausableBuildingFragment;
			_buildingSoundControllerFragment = buildingSoundControllerFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleHeaderFragment(_pausableBuildingFragment);
			builder.AddLeftHeaderFragment(_deleteBuildingFragment, 0);
			builder.AddBottomFragment(_buildingSoundControllerFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<BuildingAreaBoundsDrawingBlocker>().AsTransient();
		Bind<DeleteBuildingFragment>().AsSingleton();
		Bind<PausableBuildingFragment>().AsSingleton();
		Bind<BuildingBatchControlRowItemFactory>().AsSingleton();
		Bind<AccessibleDebugger>().AsSingleton();
		Bind<BuildingSoundControllerFragment>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<IDevModule>().To<BuildingsModelToggler>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<BuildingSpec, AreaBoundsDrawingBlocker>();
		builder.AddDecorator<BuildingSpec, BuildingAreaBoundsDrawingBlocker>();
		return builder.Build();
	}
}
