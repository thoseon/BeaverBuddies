using Bindito.Core;
using Timberborn.AlertPanelSystem;

namespace Timberborn.GameFactionSystemUI;

[Context("Game")]
internal class GameFactionSystemUIConfigurator : Configurator
{
	private class AlertPanelModuleProvider : IProvider<AlertPanelModule>
	{
		private readonly FactionUnlockedAlertFragment _factionUnlockedAlertFragment;

		public AlertPanelModuleProvider(FactionUnlockedAlertFragment factionUnlockedAlertFragment)
		{
			_factionUnlockedAlertFragment = factionUnlockedAlertFragment;
		}

		public AlertPanelModule Get()
		{
			AlertPanelModule.Builder builder = new AlertPanelModule.Builder();
			builder.AddAlertFragment(_factionUnlockedAlertFragment, 2);
			return builder.Build();
		}
	}

	protected override void Configure()
	{
		Bind<FactionUnlockedAlertFragment>().AsSingleton();
		MultiBind<AlertPanelModule>().ToProvider<AlertPanelModuleProvider>().AsSingleton();
	}
}
