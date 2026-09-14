using System;
using Steamworks;
using Timberborn.SteamStoreSystem;

namespace Timberborn.SteamWorkshop;

public class SteamWorkshopItemCreator
{
	public void CreateItem(Action<SteamWorkshopCreateResponse> createCallback)
	{
		SteamAPICall_t hAPICall = SteamUGC.CreateItem(SteamAppId.AppId, EWorkshopFileType.k_EWorkshopFileTypeFirst);
		CallResult<CreateItemResult_t>.Create().Set(hAPICall, delegate(CreateItemResult_t t, bool failure)
		{
			OnItemCreated(t, failure, createCallback);
		});
	}

	private static void OnItemCreated(CreateItemResult_t result, bool ioFailure, Action<SteamWorkshopCreateResponse> createCallback)
	{
		createCallback(new SteamWorkshopCreateResponse(result.m_nPublishedFileId.m_PublishedFileId, ioFailure ? EResult.k_EResultIOFailure : result.m_eResult));
	}
}
