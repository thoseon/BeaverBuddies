using Timberborn.EntitySystem;

namespace Timberborn.Achievements;

internal class PlugAllBadwaterSourcesAchievement : PlugBadwaterSourceAchievement
{
	public PlugAllBadwaterSourcesAchievement(EntityComponentRegistry entityComponentRegistry)
		: base(entityComponentRegistry, mustPlugAll: true, "PLUG_ALL_BADWATER_SOURCES")
	{
	}
}
