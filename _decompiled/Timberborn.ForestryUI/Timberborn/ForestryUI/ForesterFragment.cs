using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Forestry;
using UnityEngine.UIElements;

namespace Timberborn.ForestryUI;

internal class ForesterFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private VisualElement _root;

	private Toggle _toggle;

	private Forester _forester;

	public ForesterFragment(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/ForesterFragment");
		_toggle = _root.Q<Toggle>("Toggle");
		_toggle.RegisterValueChangedCallback(OnToggleValueChanged);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_forester = entity.GetComponent<Forester>();
		UpdateToggleState();
		_root.ToggleDisplayStyle(_forester);
	}

	public void UpdateFragment()
	{
		UpdateToggleState();
	}

	public void ClearFragment()
	{
		_forester = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	private void OnToggleValueChanged(ChangeEvent<bool> evt)
	{
		_forester.SetReplantDeadTrees(evt.newValue);
	}

	private void UpdateToggleState()
	{
		if ((bool)_forester)
		{
			_toggle.SetValueWithoutNotify(_forester.ReplantDeadTrees);
		}
	}
}
