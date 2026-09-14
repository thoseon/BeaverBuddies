using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.UILayoutSystem;

[Context("Game")]
[Context("MapEditor")]
internal class UILayoutConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<UIVisibilityManager>().AsSingleton();
		Bind<OverlayPanelSpeedLocker>().AsSingleton();
		Bind<UILayout>().AsSingleton();
		MultiBind<IDevModule>().To<DebugUIScaleChanger>().AsSingleton();
	}
}
