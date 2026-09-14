using System.Collections.Generic;
using Timberborn.AssetSystem;
using Timberborn.BottomBarSystem;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.Modding;
using Timberborn.Versioning;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.Options;

public class ShowOptionsButton : IBottomBarElementsProvider
{
	private static readonly string OptionsTooltipLocKey = "Tool.Options.Tooltip";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly IOptionsBox _optionsBox;

	private readonly IAssetLoader _assetLoader;

	private readonly ILoc _loc;

	private Label _tooltip;

	public ShowOptionsButton(VisualElementLoader visualElementLoader, IOptionsBox optionsBox, IAssetLoader assetLoader, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_optionsBox = optionsBox;
		_assetLoader = assetLoader;
		_loc = loc;
	}

	public IEnumerable<BottomBarElement> GetElements()
	{
		VisualElement visualElement = _visualElementLoader.LoadVisualElement("Common/BottomBar/GrouplessToolButton");
		visualElement.AddToClassList("bottom-bar-button--red");
		Sprite v = _assetLoader.Load<Sprite>("Sprites/BottomBar/Options");
		visualElement.Q<VisualElement>("ToolImage").style.backgroundImage = new StyleBackground(v);
		Button button = visualElement.Q<Button>("ToolButton");
		button.RegisterCallback<ClickEvent>(delegate
		{
			_optionsBox.Show();
		});
		button.Q<TextElement>("BottomText").text = (ModdedState.IsModded ? ("-" + GameVersions.CurrentVersion.Numeric + "-") : GameVersions.CurrentVersion.Numeric);
		_tooltip = button.Q<Label>("Tooltip");
		_tooltip.ToggleDisplayStyle(visible: false);
		_tooltip.text = _loc.T(OptionsTooltipLocKey);
		visualElement.RegisterCallback<MouseOverEvent>(ShowTooltip);
		visualElement.RegisterCallback<MouseOutEvent>(HideTooltip);
		yield return BottomBarElement.CreateSingleLevel(visualElement);
	}

	private void ShowTooltip(MouseOverEvent mouseOverEvent)
	{
		_tooltip.ToggleDisplayStyle(visible: true);
	}

	private void HideTooltip(MouseOutEvent mouseOutEvent)
	{
		_tooltip?.ToggleDisplayStyle(visible: false);
	}
}
