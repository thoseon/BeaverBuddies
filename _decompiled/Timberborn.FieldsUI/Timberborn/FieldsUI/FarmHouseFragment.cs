using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Fields;
using UnityEngine.UIElements;

namespace Timberborn.FieldsUI;

internal class FarmHouseFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly FarmHouseToggleFactory _farmHouseToggleFactory;

	private FarmHouseToggle _farmHouseToggle;

	private VisualElement _root;

	public FarmHouseFragment(VisualElementLoader visualElementLoader, FarmHouseToggleFactory farmHouseToggleFactory)
	{
		_visualElementLoader = visualElementLoader;
		_farmHouseToggleFactory = farmHouseToggleFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/FarmHouseFragment");
		_farmHouseToggle = _farmHouseToggleFactory.Create(_root);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		FarmHouse component = entity.GetComponent<FarmHouse>();
		if (component != null)
		{
			_farmHouseToggle.Show(component);
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_farmHouseToggle.Clear();
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		_farmHouseToggle.Update();
	}
}
