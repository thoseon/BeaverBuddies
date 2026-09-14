using Timberborn.EntitySystem;
using Timberborn.SingletonSystem;

namespace Timberborn.Achievements;

internal class BuildDamAchievement : BuildAchievement
{
	public BuildDamAchievement(EventBus eventBus, EntityComponentRegistry entityComponentRegistry)
		: base(eventBus, entityComponentRegistry, "BUILD_DAM", "Dam.")
	{
	}
}
