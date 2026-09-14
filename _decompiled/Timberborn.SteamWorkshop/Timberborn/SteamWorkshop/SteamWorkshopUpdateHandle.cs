using Steamworks;

namespace Timberborn.SteamWorkshop;

public class SteamWorkshopUpdateHandle
{
	private readonly UGCUpdateHandle_t _updateHandle;

	public SteamWorkshopUpdateHandle(UGCUpdateHandle_t updateHandle)
	{
		_updateHandle = updateHandle;
	}

	public float GetProgress()
	{
		if (_updateHandle == UGCUpdateHandle_t.Invalid)
		{
			return 0f;
		}
		if (SteamUGC.GetItemUpdateProgress(_updateHandle, out var punBytesProcessed, out var punBytesTotal) != EItemUpdateStatus.k_EItemUpdateStatusInvalid)
		{
			return (float)punBytesProcessed / (float)punBytesTotal;
		}
		return 0f;
	}
}
