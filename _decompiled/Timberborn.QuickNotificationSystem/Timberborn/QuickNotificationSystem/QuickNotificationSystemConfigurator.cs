using Bindito.Core;

namespace Timberborn.QuickNotificationSystem;

[Context("Game")]
[Context("MapEditor")]
internal class QuickNotificationSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<QuickNotificationService>().AsSingleton();
		Bind<QuickNotificationPanel>().AsSingleton();
	}
}
