using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.Planting;
using Timberborn.SelectionSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.ToolSystem;

namespace Timberborn.PlantingUI;

[Context("Game")]
internal class PlantingUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly PlantablePrioritizerFragment _plantablePrioritizerFragment;

		public EntityPanelModuleProvider(PlantablePrioritizerFragment plantablePrioritizerFragment)
		{
			_plantablePrioritizerFragment = plantablePrioritizerFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_plantablePrioritizerFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<PlantablePreview>().AsTransient();
		Bind<PlantablePrioritizerDropdownProvider>().AsTransient();
		Bind<PlantablePrioritizerFragment>().AsSingleton();
		Bind<PlantablePreviewService>().AsSingleton();
		Bind<PlantablePreviewFactory>().AsSingleton();
		Bind<PlantableDescriber>().AsSingleton();
		Bind<PlantingModeService>().AsSingleton();
		Bind<UnlockedPlantableService>().AsSingleton();
		Bind<PlantingSelectionService>().AsSingleton();
		Bind<DevModePlantableSpawner>().AsSingleton();
		Bind<PlantingToolButtonFactory>().AsSingleton();
		Bind<PlantablePrioritizerBatchControlRowItemFactory>().AsSingleton();
		Bind<UnlockedPlantableGroupsRegistry>().AsSingleton();
		MultiBind<IToolFinder>().To<PlantingToolFinder>().AsSingleton();
		MultiBind<IToolLocker>().To<PlantableToolLocker>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<PlanterBuilding, PlantablePrioritizerDropdownProvider>();
		builder.AddDecorator<PlantablePreviewSpec, PlantablePreview>();
		builder.AddDecorator<PlantablePreview, HighlightableObject>();
		return builder.Build();
	}
}
