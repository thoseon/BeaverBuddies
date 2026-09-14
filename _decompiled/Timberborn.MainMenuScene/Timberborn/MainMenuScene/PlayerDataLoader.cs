using System;
using System.Text;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.PlatformUtilities;
using Timberborn.PlayerDataSystem;
using UnityEngine.UIElements;

namespace Timberborn.MainMenuScene;

internal class PlayerDataLoader
{
	private static readonly string CorruptedDataLocKey = "PlayerData.CorruptedDataInfo.Header";

	private readonly IPlayerDataService _playerDataService;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly VisualElementLoader _visualElementLoader;

	private readonly IExplorerOpener _explorerOpener;

	private readonly ILoc _loc;

	private readonly HyperlinkInitializer _hyperlinkInitializer;

	public PlayerDataLoader(IPlayerDataService playerDataService, DialogBoxShower dialogBoxShower, VisualElementLoader visualElementLoader, IExplorerOpener explorerOpener, ILoc loc, HyperlinkInitializer hyperlinkInitializer)
	{
		_playerDataService = playerDataService;
		_dialogBoxShower = dialogBoxShower;
		_visualElementLoader = visualElementLoader;
		_explorerOpener = explorerOpener;
		_loc = loc;
		_hyperlinkInitializer = hyperlinkInitializer;
	}

	public void Load(Action nextAction)
	{
		if (_playerDataService.DataLoadSuccessful)
		{
			nextAction();
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(_loc.T(CorruptedDataLocKey));
		string elementName = "MainMenu/CorruptedPlayerDataLabel";
		Label label = _visualElementLoader.LoadVisualElement(elementName).Q<Label>();
		_hyperlinkInitializer.Initialize(label, delegate
		{
			_explorerOpener.OpenDirectory(PlayerDataFileService.PlayerDataDirectory);
		});
		_dialogBoxShower.Create().SetMessage(stringBuilder.ToString().TrimEnd()).AddContent(label)
			.SetConfirmButton(nextAction)
			.Show();
	}
}
