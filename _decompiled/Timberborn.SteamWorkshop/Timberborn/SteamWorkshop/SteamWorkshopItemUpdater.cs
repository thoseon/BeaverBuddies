using System;
using System.Linq;
using Steamworks;
using Timberborn.SteamStoreSystem;

namespace Timberborn.SteamWorkshop;

public class SteamWorkshopItemUpdater
{
	public SteamWorkshopUpdateHandle Update(SteamWorkshopUpdateRequest request, Action<SteamWorkshopUpdateResponse> updateCallback)
	{
		UGCUpdateHandle_t uGCUpdateHandle_t = SteamUGC.StartItemUpdate(SteamAppId.AppId, new PublishedFileId_t(request.ItemId));
		SetUpdateContent(request, uGCUpdateHandle_t);
		SteamAPICall_t hAPICall = SteamUGC.SubmitItemUpdate(uGCUpdateHandle_t, request.Changelog);
		CallResult<SubmitItemUpdateResult_t>.Create().Set(hAPICall, delegate(SubmitItemUpdateResult_t t, bool failure)
		{
			OnItemUpdated(t, failure, updateCallback, request);
		});
		return new SteamWorkshopUpdateHandle(uGCUpdateHandle_t);
	}

	private static void SetUpdateContent(SteamWorkshopUpdateRequest updateRequest, UGCUpdateHandle_t updateHandle)
	{
		if (!string.IsNullOrEmpty(updateRequest.Name))
		{
			SteamUGC.SetItemTitle(updateHandle, updateRequest.Name);
		}
		if (!string.IsNullOrEmpty(updateRequest.Description))
		{
			SteamUGC.SetItemDescription(updateHandle, updateRequest.Description);
		}
		if (updateRequest.Visibility.HasValue)
		{
			SteamUGC.SetItemVisibility(updateHandle, updateRequest.Visibility.Value.ToStorageVisibility());
		}
		if (updateRequest.MandatoryTags.Length > 0 || updateRequest.ChosenTags.Length > 0)
		{
			SteamUGC.SetItemTags(updateHandle, updateRequest.MandatoryTags.Concat(updateRequest.ChosenTags).ToArray());
		}
		if (!string.IsNullOrEmpty(updateRequest.PreviewPath))
		{
			SteamUGC.SetItemPreview(updateHandle, updateRequest.PreviewPath);
		}
		if (!string.IsNullOrEmpty(updateRequest.ContentPath))
		{
			SteamUGC.SetItemContent(updateHandle, updateRequest.ContentPath);
		}
	}

	private static void OnItemUpdated(SubmitItemUpdateResult_t result, bool ioFailure, Action<SteamWorkshopUpdateResponse> updateCallback, SteamWorkshopUpdateRequest request)
	{
		updateCallback(new SteamWorkshopUpdateResponse(request, ioFailure ? EResult.k_EResultIOFailure : result.m_eResult));
	}
}
