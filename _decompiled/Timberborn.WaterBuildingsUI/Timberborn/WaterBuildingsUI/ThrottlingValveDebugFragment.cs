using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.WaterBuildings;
using UnityEngine.UIElements;

namespace Timberborn.WaterBuildingsUI;

internal class ThrottlingValveDebugFragment : IEntityPanelFragment
{
	private readonly DebugFragmentFactory _debugFragmentFactory;

	private readonly ILoc _loc;

	private readonly Phrase _currentOutflowLimitPhrase = Phrase.New().Format((float value) => $"Current outflow limit: {value:F4}cms");

	private ThrottlingValve _throttlingValve;

	private VisualElement _root;

	private Label _text;

	public ThrottlingValveDebugFragment(DebugFragmentFactory debugFragmentFactory, ILoc loc)
	{
		_debugFragmentFactory = debugFragmentFactory;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _debugFragmentFactory.Create("ThrottlingValve");
		_text = _root.Q<Label>("Text");
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_throttlingValve = entity.GetComponent<ThrottlingValve>();
	}

	public void ClearFragment()
	{
		_throttlingValve = null;
		UpdateFragment();
	}

	public void UpdateFragment()
	{
		if ((bool)_throttlingValve)
		{
			_text.text = (_throttlingValve.CurrentOutflowLimit.HasValue ? _loc.T(_currentOutflowLimitPhrase, _throttlingValve.CurrentOutflowLimit.Value) : "Unlimited");
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}
}
