using System.Collections.Generic;
using System.Collections.Immutable;
using Timberborn.SingletonSystem;

namespace Timberborn.AchievementSystem;

internal class AchievementService : IPostLoadableSingleton
{
	private readonly IStoreAchievements _storeAchievements;

	private readonly ImmutableArray<Achievement> _achievements;

	public AchievementService(IStoreAchievements storeAchievements, IEnumerable<Achievement> achievements)
	{
		_storeAchievements = storeAchievements;
		_achievements = achievements.ToImmutableArray();
	}

	public void PostLoad()
	{
		foreach (Achievement achievement in GetLockedAchievements())
		{
			achievement.Enable(delegate
			{
				UnlockAchievement(achievement.Id);
			});
		}
	}

	private IEnumerable<Achievement> GetLockedAchievements()
	{
		foreach (Achievement achievement in _achievements)
		{
			if (!_storeAchievements.IsAchievementUnlocked(achievement.Id))
			{
				yield return achievement;
			}
		}
	}

	private void UnlockAchievement(string achievementId)
	{
		_storeAchievements.UnlockAchievement(achievementId);
	}
}
