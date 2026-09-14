using Bindito.Core;
using Timberborn.Debugging;
using Timberborn.EntityPanelSystem;

namespace Timberborn.InventorySystemUI;

[Context("Game")]
internal class InventorySystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly InventoryDebugFragment _inventoryDebugFragment;

		public EntityPanelModuleProvider(InventoryDebugFragment inventoryDebugFragment)
		{
			_inventoryDebugFragment = inventoryDebugFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddDiagnosticFragment(_inventoryDebugFragment);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<InventoryDebugFragment>().AsSingleton();
		Bind<ModifyInventoryBox>().AsSingleton();
		Bind<InformationalRowsFactory>().AsSingleton();
		Bind<InventoryFragmentBuilderFactory>().AsSingleton();
		Bind<InventoryRowUpdater>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<IDevModule>().To<InventoryFillerDevModule>().AsSingleton();
	}
}
