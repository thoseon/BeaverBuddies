using Bindito.Core;

namespace Timberborn.SoundSystem;

[Context("MainMenu")]
[Context("Game")]
[Context("MapEditor")]
internal class SoundSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AudioClipService>().AsSingleton();
		Bind<ISoundSystem>().To<SoundSystem>().AsSingleton();
		Bind<AudioMixerGroupRetriever>().AsSingleton();
		Bind<VolumeController>().AsSingleton();
		Bind<AudioSourceFactory>().AsSingleton();
		Bind<AudioSourceFader>().AsSingleton();
		Bind<SoundEmitterRetriever>().AsSingleton();
	}
}
