using Bindito.Core;

namespace Timberborn.AccessibilitySettingsSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class AccessibilitySettingsSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AccessibilitySettings>().AsSingleton();
	}
}
