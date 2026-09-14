using Steamworks;

namespace Timberborn.SteamWorkshop;

public class SteamWorkshopCreateResponse
{
	public ulong ItemId { get; }

	public EResult Result { get; }

	public bool Successful => Result == EResult.k_EResultOK;

	public string ResultMessage => $"{Result.ToString()} ({(int)Result})";

	public SteamWorkshopCreateResponse(ulong itemId, EResult result)
	{
		ItemId = itemId;
		Result = result;
	}
}
