using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class BuildCampfireAchievement : BuildAchievement
{
	public BuildCampfireAchievement(EventBus eventBus, EntityComponentRegistry entityComponentRegistry)
		: base(eventBus, entityComponentRegistry, "BUILD_CAMPFIRE", "Campfire.")
	{
	}
}
