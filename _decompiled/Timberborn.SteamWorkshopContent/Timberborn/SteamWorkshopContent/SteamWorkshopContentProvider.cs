using System.Collections.Generic;
using System.IO;
using Steamworks;
using Timberborn.SteamStoreSystem;

namespace Timberborn.SteamWorkshopContent;

public class SteamWorkshopContentProvider
{
	private static readonly uint PathBufferSize = 1024u;

	private readonly SteamManager _steamManager;

	public SteamWorkshopContentProvider(SteamManager steamManager)
	{
		_steamManager = steamManager;
	}

	public IEnumerable<DirectoryInfo> GetContentDirectories()
	{
		if (!_steamManager.Initialized)
		{
			yield break;
		}
		foreach (PublishedFileId_t subscribedItem in GetSubscribedItems())
		{
			if (SteamUGC.GetItemInstallInfo(subscribedItem, out var _, out var pchFolder, PathBufferSize, out var _))
			{
				DirectoryInfo directoryInfo = new DirectoryInfo(pchFolder);
				if (directoryInfo.Exists)
				{
					yield return directoryInfo;
				}
			}
		}
	}

	private static IEnumerable<PublishedFileId_t> GetSubscribedItems()
	{
		uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();
		PublishedFileId_t[] array = new PublishedFileId_t[numSubscribedItems];
		SteamUGC.GetSubscribedItems(array, numSubscribedItems);
		return array;
	}
}
