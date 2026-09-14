using Bindito.Core;
using Timberborn.EntityPanelSystem;
using Timberborn.Stockpiles;
using Timberborn.TemplateInstantiation;

namespace Timberborn.MapEditorStockpilesUI;

[Context("MapEditor")]
internal class MapEditorStockpilesUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly FixedStockpileFragment _fixedStockpileFragment;

		public EntityPanelModuleProvider(FixedStockpileFragment fixedStockpileFragment)
		{
			_fixedStockpileFragment = fixedStockpileFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddBottomFragment(_fixedStockpileFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<FixedStockpileDropdownProvider>().AsTransient();
		Bind<FixedStockpileInventorySetter>().AsTransient();
		Bind<FixedStockpileFragment>().AsSingleton();
		Bind<FixedStockpileGoodProvider>().AsSingleton();
		MultiBind<TemplateModule>().ToProvider(ProvideTemplateModule).AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}

	private static TemplateModule ProvideTemplateModule()
	{
		TemplateModule.Builder builder = new TemplateModule.Builder();
		builder.AddDecorator<Stockpile, FixedStockpileDropdownProvider>();
		builder.AddDecorator<Stockpile, FixedStockpileInventorySetter>();
		return builder.Build();
	}
}
