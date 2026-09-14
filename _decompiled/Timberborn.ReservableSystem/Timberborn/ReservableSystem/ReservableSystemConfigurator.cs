using Bindito.Core;

namespace Timberborn.ReservableSystem;

[Context("Game")]
[Context("MapEditor")]
internal class ReservableSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WalkToReservableExecutor>().AsTransient();
		Bind<Reservable>().AsTransient();
	}
}
