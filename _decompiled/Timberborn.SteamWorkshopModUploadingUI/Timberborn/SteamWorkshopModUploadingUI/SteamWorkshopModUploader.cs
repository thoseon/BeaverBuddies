using Timberborn.Localization;
using Timberborn.MainMenuModdingUI;
using Timberborn.Modding;
using Timberborn.SingletonSystem;
using Timberborn.SteamStoreSystem;
using Timberborn.SteamWorkshopUI;

namespace Timberborn.SteamWorkshopModUploadingUI;

internal class SteamWorkshopModUploader : ILoadableSingleton
{
	private static readonly string UploadLocKey = "SteamWorkshop.UploadToSteamWorkshop";

	private readonly SteamWorkshopUploadPanel _steamWorkshopUploadPanel;

	private readonly SteamWorkshopUploadableModFactory _steamWorkshopUploadableModFactory;

	private readonly ModUploaderBox _modUploaderBox;

	private readonly ILoc _loc;

	private readonly SteamManager _steamManager;

	public SteamWorkshopModUploader(SteamWorkshopUploadPanel steamWorkshopUploadPanel, SteamWorkshopUploadableModFactory steamWorkshopUploadableModFactory, ModUploaderBox modUploaderBox, ILoc loc, SteamManager steamManager)
	{
		_steamWorkshopUploadPanel = steamWorkshopUploadPanel;
		_steamWorkshopUploadableModFactory = steamWorkshopUploadableModFactory;
		_modUploaderBox = modUploaderBox;
		_loc = loc;
		_steamManager = steamManager;
	}

	public void Load()
	{
		if (_steamManager.Initialized)
		{
			_modUploaderBox.AddUploader(_loc.T(UploadLocKey), OpenUploadPanel);
		}
	}

	private void OpenUploadPanel(Mod mod)
	{
		_steamWorkshopUploadPanel.Open(_steamWorkshopUploadableModFactory.Create(mod));
	}
}
