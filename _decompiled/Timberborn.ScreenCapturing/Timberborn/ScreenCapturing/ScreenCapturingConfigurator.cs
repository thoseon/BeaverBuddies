using Bindito.Core;

namespace Timberborn.ScreenCapturing;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class ScreenCapturingConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ScreenshotService>().AsSingleton();
	}
}
