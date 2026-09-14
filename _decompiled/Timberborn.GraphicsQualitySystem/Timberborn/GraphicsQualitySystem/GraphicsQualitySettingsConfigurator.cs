using Bindito.Core;

namespace Timberborn.GraphicsQualitySystem;

[Context("Game")]
[Context("MainMenu")]
[Context("MapEditor")]
internal class GraphicsQualitySettingsConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AnisotropicFilteringSetting>().AsSingleton();
		Bind<AntiAliasingTypeSetting>().AsSingleton();
		Bind<GraphicsQualitySettings>().AsSingleton().AsExported();
		Bind<LightQualitySetting>().AsSingleton();
		Bind<ShadowQualityGraphicsSettings>().AsSingleton();
		Bind<TextureQualitySetting>().AsSingleton();
		Bind<WaterQualitySetting>().AsSingleton();
		Bind<BloomSetting>().AsSingleton();
	}
}
