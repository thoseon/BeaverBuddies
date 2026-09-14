namespace Timberborn.AchievementSystem;

public interface IStoreAchievements
{
	bool IsAchievementUnlocked(string achievementId);

	void UnlockAchievement(string achievementId);
}
