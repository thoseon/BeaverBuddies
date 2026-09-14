using Bindito.Core;

namespace Timberborn.CameraSettingsSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class CameraSettingsSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<CameraSettings>().AsSingleton();
	}
}
