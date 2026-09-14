using Bindito.Core;

namespace Timberborn.LevelVisibilitySystem;

[Context("Game")]
[Context("MapEditor")]
internal class LevelVisibilitySystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<ILevelVisibilityService>().To<LevelVisibilityService>().AsSingleton();
	}
}
