using Bindito.Core;

namespace Timberborn.NotificationSystemUI;

[Context("Game")]
internal class NotificationSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<NotificationPanel>().AsSingleton();
	}
}
