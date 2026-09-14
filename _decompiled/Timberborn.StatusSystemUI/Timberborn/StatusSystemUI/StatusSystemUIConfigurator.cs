using Bindito.Core;
using Timberborn.AlertPanelSystem;
using Timberborn.Debugging;
using Timberborn.EntityPanelSystem;

namespace Timberborn.StatusSystemUI;

[Context("Game")]
internal class StatusSystemUIConfigurator : Configurator
{
	private class EntityPanelModuleProvider : IProvider<EntityPanelModule>
	{
		private readonly StatusListFragment _statusListFragment;

		public EntityPanelModuleProvider(StatusListFragment statusListFragment)
		{
			_statusListFragment = statusListFragment;
		}

		public EntityPanelModule Get()
		{
			EntityPanelModule.Builder builder = new EntityPanelModule.Builder();
			builder.AddFooterFragment(_statusListFragment);
			return builder.Build();
		}
	}

	private class AlertPanelModuleProvider : IProvider<AlertPanelModule>
	{
		private readonly StatusAlertFragment _statusAlertFragment;

		private readonly DynamicStatusAlertFragment _dynamicStatusAlertFragment;

		public AlertPanelModuleProvider(StatusAlertFragment statusAlertFragment, DynamicStatusAlertFragment dynamicStatusAlertFragment)
		{
			_statusAlertFragment = statusAlertFragment;
			_dynamicStatusAlertFragment = dynamicStatusAlertFragment;
		}

		public AlertPanelModule Get()
		{
			AlertPanelModule.Builder builder = new AlertPanelModule.Builder();
			builder.AddAlertFragment(_statusAlertFragment, 4);
			builder.AddAlertFragment(_dynamicStatusAlertFragment, 3);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<StatusAlertFragmentRowFactory>().AsTransient();
		Bind<StatusBatchControlRowItemFactory>().AsSingleton();
		Bind<StatusListFragment>().AsSingleton();
		Bind<StatusAlertFragment>().AsSingleton();
		Bind<DynamicStatusAlertFragment>().AsSingleton();
		Bind<StatusAlertRowBlinker>().AsSingleton();
		MultiBind<EntityPanelModule>().ToProvider<EntityPanelModuleProvider>().AsSingleton();
		MultiBind<AlertPanelModule>().ToProvider<AlertPanelModuleProvider>().AsSingleton();
		MultiBind<IDevModule>().To<StatusSystemSlotsDrawer>().AsSingleton();
	}
}
