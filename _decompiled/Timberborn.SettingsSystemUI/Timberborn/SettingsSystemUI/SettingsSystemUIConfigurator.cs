using Bindito.Core;

namespace Timberborn.SettingsSystemUI;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class SettingsSystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AnalyticsSettingsController>().AsSingleton();
		Bind<AnisotropicFilteringDropdownProvider>().AsSingleton();
		Bind<AntiAliasingDropdownProvider>().AsSingleton();
		Bind<DevModeSettingsController>().AsSingleton();
		Bind<FrameRateLimitDropdownProvider>().AsSingleton();
		Bind<GameSavingSettings>().AsSingleton();
		Bind<GameSavingSettingsController>().AsSingleton();
		Bind<GraphicsSettingsController>().AsSingleton();
		Bind<GraphicsQualityDropdownProvider>().AsSingleton();
		Bind<ISettingsController>().To<SettingsBox>().AsSingleton();
		Bind<InputSettingsController>().AsSingleton();
		Bind<LanguageSettingsController>().AsSingleton();
		Bind<LightQualityDropdownProvider>().AsSingleton();
		Bind<OnScreenKeyboardDropdownProvider>().AsSingleton();
		Bind<ScreenResolutionDropdownProvider>().AsSingleton();
		Bind<ScreenSettingsController>().AsSingleton();
		Bind<ShadowQualityGraphicsDropdownProvider>().AsSingleton();
		Bind<SoundSettingsController>().AsSingleton();
		Bind<TextureQualityDropdownProvider>().AsSingleton();
		Bind<TutorialSettingsController>().AsSingleton();
		Bind<UISettingsController>().AsSingleton();
		Bind<VSyncDropdownProvider>().AsSingleton();
		Bind<AccessibilitySettingsController>().AsSingleton();
		Bind<WaterQualityDropdownProvider>().AsSingleton();
		Bind<BloomDropdownProvider>().AsSingleton();
		Bind<CameraSettingsController>().AsSingleton();
	}
}
