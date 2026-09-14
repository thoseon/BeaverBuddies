using Bindito.Core;
using Timberborn.EntityPanelSystem;

namespace Timberborn.StockpileVisualizationUI;

[Context("Game")]
internal class StockpileVisualizationUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly StockpileGoodColumnVisualizerDebugFragment _stockpileGoodColumnVisualizerDebugFragment;

		public EntityPanelModuleProvider(StockpileGoodColumnVisualizerDebugFragment stockpileGoodColumnVisualizerDebugFragment)
		{
			_stockpileGoodColumnVisualizerDebugFragment = stockpileGoodColumnVisualizerDebugFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddDiagnosticFragment(_stockpileGoodColumnVisualizerDebugFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<StockpileGoodColumnVisualizerDebugFragment>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
	}
}
