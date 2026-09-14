using Timberborn.CameraSystem;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.SingletonSystem;
using Timberborn.UIFormatters;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.PowerGenerationUI;

internal class WaterPoweredGeneratorPreviewPanel : ILoadableSingleton
{
	private static readonly string PowerClass = "square-large--brown";

	private static readonly string NoPowerClass = "square-large--light-red";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly CameraService _cameraService;

	private readonly Underlay _underlay;

	private readonly ILoc _loc;

	private VisualElement _root;

	private Label _outputPower;

	private bool _isVisible;

	private readonly Phrase _outputPowerPhrase = Phrase.New().FormatPower<int>();

	public WaterPoweredGeneratorPreviewPanel(VisualElementLoader visualElementLoader, CameraService cameraService, Underlay underlay, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_cameraService = cameraService;
		_underlay = underlay;
		_loc = loc;
	}

	public void Load()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/WaterPoweredGeneratorPreviewPanel");
		_outputPower = _root.Q<Label>("Text");
		_underlay.Add(_root);
		_root.ToggleDisplayStyle(visible: false);
	}

	public void ShowPreview(int powerOutput, Vector3 position)
	{
		if (!_isVisible)
		{
			_root.ToggleDisplayStyle(visible: true);
			_underlay.Add(_root);
			_isVisible = true;
		}
		_outputPower.text = _loc.T(_outputPowerPhrase, powerOutput);
		_root.EnableInClassList(PowerClass, powerOutput > 0);
		_root.EnableInClassList(NoPowerClass, powerOutput == 0);
		UpdateRootPosition(position);
	}

	public void HidePreview()
	{
		if (_isVisible)
		{
			_root.ToggleDisplayStyle(visible: false);
			_underlay.Remove(_root);
			_isVisible = false;
		}
	}

	private void UpdateRootPosition(Vector3 position)
	{
		if (_root.panel != null)
		{
			bool flag = _cameraService.IsInFront(position);
			_root.ToggleDisplayStyle(flag);
			if (flag)
			{
				VisualElement root = _underlay.Root;
				Vector3 vector = _cameraService.WorldSpaceToPanelSpace(root, position);
				_root.style.translate = new Vector2(vector.x - root.layout.width / 2f, vector.y - root.layout.height / 2f);
			}
		}
	}
}
