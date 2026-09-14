using Bindito.Core;

namespace Timberborn.NotificationSystem;

[Context("Game")]
internal class NotificationSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<NotificationBus>().AsSingleton();
		Bind<NotificationValueSerializer>().AsSingleton();
		Bind<NotificationSaver>().AsSingleton();
	}
}
