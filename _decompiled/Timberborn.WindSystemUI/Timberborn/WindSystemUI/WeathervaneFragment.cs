using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.WindSystem;
using UnityEngine.UIElements;

namespace Timberborn.WindSystemUI;

public class WeathervaneFragment : IEntityPanelFragment
{
	private static readonly string WindStrengthLocKey = "Building.Weathervane.WindStrength";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private readonly WindService _windService;

	private Label _windStrengthLabel;

	private VisualElement _root;

	private WeathervaneSpec _weathervaneSpec;

	private BlockObject _blockObject;

	public WeathervaneFragment(VisualElementLoader visualElementLoader, ILoc loc, WindService windService)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
		_windService = windService;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/WeathervaneFragment");
		_root.ToggleDisplayStyle(visible: false);
		_windStrengthLabel = _root.Q<Label>("WindStrengthLabel");
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_weathervaneSpec = entity.GetComponent<WeathervaneSpec>();
		if (_weathervaneSpec != null)
		{
			_blockObject = entity.GetComponent<BlockObject>();
		}
	}

	public void ClearFragment()
	{
		_weathervaneSpec = null;
		_blockObject = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if (_weathervaneSpec != null && (bool)_blockObject && _blockObject.IsFinished)
		{
			float num = _windService.WindStrength * 100f;
			_root.ToggleDisplayStyle(visible: true);
			_windStrengthLabel.text = _loc.T(WindStrengthLocKey, num.ToString("0"));
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}
}
