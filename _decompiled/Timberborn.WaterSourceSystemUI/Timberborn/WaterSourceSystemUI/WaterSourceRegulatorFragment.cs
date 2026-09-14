using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.WaterSourceSystem;
using UnityEngine.UIElements;

namespace Timberborn.WaterSourceSystemUI;

internal class WaterSourceRegulatorFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly WaterSourceRegulatorToggleFactory _waterSourceRegulatorToggleFactory;

	private VisualElement _root;

	private WaterSourceRegulatorToggle _waterSourceRegulatorToggle;

	public WaterSourceRegulatorFragment(VisualElementLoader visualElementLoader, WaterSourceRegulatorToggleFactory waterSourceRegulatorToggleFactory)
	{
		_visualElementLoader = visualElementLoader;
		_waterSourceRegulatorToggleFactory = waterSourceRegulatorToggleFactory;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/WaterSourceRegulatorFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		VisualElement parent = _root.Q<VisualElement>("ModeToggle");
		Label label = _root.Q<Label>("ModeLabel");
		_waterSourceRegulatorToggle = _waterSourceRegulatorToggleFactory.Create(parent, label);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		WaterSourceRegulator component = entity.GetComponent<WaterSourceRegulator>();
		if (component != null)
		{
			_waterSourceRegulatorToggle.Show(component);
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_waterSourceRegulatorToggle.Clear();
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		_waterSourceRegulatorToggle.Update();
	}
}
