using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.GameSound;

[Context("Game")]
internal class GameSoundConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<WaterAmbientSound>().AsSingleton();
		Bind<SoundListenerDebugger>().AsSingleton();
		Bind<GameUISoundController>().AsSingleton();
		Bind<SoundSystemDebuggingPanel>().AsSingleton();
		Bind<GameMusicPlayer>().AsSingleton();
		Bind<DayNightAmbientSound>().AsSingleton();
		MultiBind<IDevModule>().To<SoundListenerDebuggerDevModule>().AsSingleton();
	}
}
