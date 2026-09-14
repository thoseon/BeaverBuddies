using Bindito.Core;

namespace Timberborn.AchievementSystem;

[Context("Game")]
internal class AchievementSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<AchievementService>().AsSingleton();
	}
}
