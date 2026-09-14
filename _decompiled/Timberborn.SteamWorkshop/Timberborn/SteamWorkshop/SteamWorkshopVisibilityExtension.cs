using System;
using Steamworks;

namespace Timberborn.SteamWorkshop;

public static class SteamWorkshopVisibilityExtension
{
	public static ERemoteStoragePublishedFileVisibility ToStorageVisibility(this SteamWorkshopVisibility visibility)
	{
		return visibility switch
		{
			SteamWorkshopVisibility.Private => ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate, 
			SteamWorkshopVisibility.Friends => ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityFriendsOnly, 
			SteamWorkshopVisibility.Public => ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic, 
			SteamWorkshopVisibility.Unlisted => ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityUnlisted, 
			_ => throw new ArgumentOutOfRangeException("visibility", visibility, null), 
		};
	}
}
