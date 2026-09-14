using Steamworks;

namespace Timberborn.SteamWorkshop;

public class SteamWorkshopUpdateResponse
{
	public SteamWorkshopUpdateRequest Request { get; }

	public EResult Result { get; }

	public bool Successful => Result == EResult.k_EResultOK;

	public string ResultMessage => $"{Result.ToString()} ({(int)Result})";

	public SteamWorkshopUpdateResponse(SteamWorkshopUpdateRequest request, EResult result)
	{
		Request = request;
		Result = result;
	}
}
