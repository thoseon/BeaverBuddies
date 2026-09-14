using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.TemplateInstantiation;
using Timberborn.Workshops;

namespace Timberborn.WorkshopsUI;

[Context("Game")]
internal class WorkshopsUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly ManufactoryFragment _manufactoryFragment;

		private readonly ManufactoryTogglableRecipesFragment _manufactoryTogglableRecipesFragment;

		private readonly ProductionProgressFragment _productionProgressFragment;

		private readonly ProductivityFragment _productivityFragment;

		private readonly ManufactoryInventoryFragment _manufactoryInventoryFragment;

		public EntityPanelModuleProvider(ManufactoryFragment manufactoryFragment, ManufactoryTogglableRecipesFragment manufactoryTogglableRecipesFragment, ProductionProgressFragment productionProgressFragment, ProductivityFragment productivityFragment, ManufactoryInventoryFragment manufactoryInventoryFragment)
		{
			_manufactoryFragment = manufactoryFragment;
			_manufactoryTogglableRecipesFragment = manufactoryTogglableRecipesFragment;
			_productionProgressFragment = productionProgressFragment;
			_productivityFragment = productivityFragment;
			_manufactoryInventoryFragment = manufactoryInventoryFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddMiddleFragment(_manufactoryFragment);
			builder.AddMiddleFragment(_manufactoryTogglableRecipesFragment);
			builder.AddMiddleFragment(_manufactoryInventoryFragment, 20);
			builder.AddMiddleFragment(_productionProgressFragment);
			builder.AddMiddleFragment(_productivityFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<ManufactoryDescriber>().AsTransient();
		Bind<ManufactoryDropdownProvider>().AsTransient();
		Bind<ProductionProgressFragment>().AsSingleton();
		Bind<ManufactoryFragment>().AsSingleton();
		Bind<ManufactoryInventoryFragment>().AsSingleton();
		Bind<ManufactoryBatchControlRowItemFactory>().AsSingleton();
		Bind<ProductivityBatchControlRowItemFactory>().AsSingleton();
		Bind<ManufactoryRecipeSliderToggleFactory>().AsSingleton();
		Bind<ManufactoryTogglableRecipesFragment>().AsSingleton();
		Bind<ManufactoryTogglableRecipesBatchControlRowItemFactory>().AsSingleton();
		Bind<ProductivityFragment>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Manufactory, ManufactoryDescriber>();
		builder.AddDecorator<Manufactory, ManufactoryDropdownProvider>();
		return builder.Build();
	}
}
