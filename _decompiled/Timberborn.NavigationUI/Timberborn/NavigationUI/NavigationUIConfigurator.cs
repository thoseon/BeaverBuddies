using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.NavigationUI;

[Context("Game")]
internal class NavigationUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<NavigationDebuggingPanel>().AsSingleton();
		MultiBind<IDevModule>().To<NavMeshDrawerController>().AsSingleton();
	}
}
