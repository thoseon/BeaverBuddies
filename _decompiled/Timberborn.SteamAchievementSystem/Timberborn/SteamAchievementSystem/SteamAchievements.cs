using System;
using Steamworks;
using Timberborn.AchievementSystem;
using Timberborn.SteamStoreSystem;
using UnityEngine;

namespace Timberborn.SteamAchievementSystem;

internal class SteamAchievements : IStoreAchievements
{
	private readonly SteamManager _steamManager;

	private Action _initializationSuccessCallback;

	public SteamAchievements(SteamManager steamManager)
	{
		_steamManager = steamManager;
	}

	public bool IsAchievementUnlocked(string achievementId)
	{
		bool pbAchieved = default(bool);
		return (_steamManager.Initialized && SteamUserStats.GetAchievement(achievementId, out pbAchieved)) & pbAchieved;
	}

	public void UnlockAchievement(string achievementId)
	{
		if (_steamManager.Initialized)
		{
			if (SteamUserStats.SetAchievement(achievementId))
			{
				SteamUserStats.StoreStats();
			}
			else
			{
				Debug.LogError("Failed to unlock achievement: " + achievementId + ".");
			}
		}
	}
}
