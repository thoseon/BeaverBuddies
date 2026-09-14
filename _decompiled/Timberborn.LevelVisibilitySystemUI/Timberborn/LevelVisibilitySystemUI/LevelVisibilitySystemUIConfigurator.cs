using Bindito.Core;
using Timberborn.Debugging;

namespace Timberborn.LevelVisibilitySystemUI;

[Context("Game")]
[Context("MapEditor")]
internal class LevelVisibilitySystemUIConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ILevelVisibilityPanel>().To<LevelVisibilityPanel>().AsSingleton();
		Bind<LevelVisibilitySelector>().AsSingleton();
		Bind<LevelVisibilityPicker>().AsSingleton();
		MultiBind<IDevModule>().To<LevelVisibilityDevModule>().AsSingleton();
	}
}
