using Timberborn.AutomationBuildings;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.AutomationBuildingsUI;

internal class GateFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly GateToggleFactory _gateToggleFactory;

	private VisualElement _root;

	private GateToggle _gateToggle;

	public GateFragment(VisualElementLoader visualElementLoader, GateToggleFactory gateToggleFactory)
	{
		_visualElementLoader = visualElementLoader;
		_gateToggleFactory = gateToggleFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/GateFragment");
		VisualElement parent = _root.Q<VisualElement>("ModeToggle");
		Label label = _root.Q<Label>("ModeLabel");
		_gateToggle = _gateToggleFactory.Create(parent, label);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		Gate component = entity.GetComponent<Gate>();
		if (component != null)
		{
			_gateToggle.Show(component);
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_gateToggle.Clear();
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		_gateToggle.Update();
	}
}
