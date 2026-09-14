using Timberborn.BuildingAvailability;
using Timberborn.EntitySystem;
using Timberborn.GameFactionSystem;
using Timberborn.SingletonSystem;
using Timberborn.TemplateSystem;
using Timberborn.WorldPersistence;

namespace Timberborn.Achievements;

internal class BuildEveryStructureFolktailsAchievement : BuildEveryStructureAchievement
{
	public BuildEveryStructureFolktailsAchievement(ISingletonLoader singletonLoader, EventBus eventBus, FactionService factionService, TemplateService templateService, EntityComponentRegistry entityComponentRegistry, BuildingAvailabilityValidator buildingAvailabilityValidator)
		: base(singletonLoader, eventBus, factionService, templateService, entityComponentRegistry, buildingAvailabilityValidator, AchievementHelper.Folktails)
	{
	}
}
