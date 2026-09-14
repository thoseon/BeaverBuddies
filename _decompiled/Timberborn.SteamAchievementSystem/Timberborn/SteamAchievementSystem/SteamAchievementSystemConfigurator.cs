using Bindito.Core;
using Timberborn.AchievementSystem;

namespace Timberborn.SteamAchievementSystem;

[Context("Game")]
internal class SteamAchievementSystemConfigurator : Configurator
{
	protected override void Configure()
	{
		Bind<IStoreAchievements>().To<SteamAchievements>().AsSingleton();
	}
}
