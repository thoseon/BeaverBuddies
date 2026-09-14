using Bindito.Core;

namespace Timberborn.AlertPanelSystem;

[Context("Game")]
[Context("MapEditor")]
internal class AlertPanelSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AlertPanel>().AsSingleton();
		Bind<AlertPanelRowFactory>().AsSingleton();
	}
}
