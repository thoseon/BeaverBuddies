using Timberborn.EntitySystem;

namespace Timberborn.Achievements;

internal class PlugAnyBadwaterSourceAchievement : PlugBadwaterSourceAchievement
{
	public PlugAnyBadwaterSourceAchievement(EntityComponentRegistry entityComponentRegistry)
		: base(entityComponentRegistry, mustPlugAll: false, "PLUG_ANY_BADWATER_SOURCE")
	{
	}
}
