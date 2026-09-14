using Timberborn.BaseComponentSystem;
using Timberborn.Buildings;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.BuildingsUI;

internal class BuildingSoundControllerFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private VisualElement _root;

	private Toggle _toggle;

	private BuildingSoundController _buildingSoundController;

	public BuildingSoundControllerFragment(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/BuildingSoundControllerFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_toggle = _root.Q<Toggle>("Toggle");
		_toggle.RegisterValueChangedCallback(ToggleSound);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		BuildingSoundController component = entity.GetComponent<BuildingSoundController>();
		if (component != null)
		{
			_buildingSoundController = component;
			_root.ToggleDisplayStyle(visible: true);
			_toggle.SetValueWithoutNotify(component.PlaySound);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	public void ClearFragment()
	{
		_buildingSoundController = null;
	}

	public void UpdateFragment()
	{
	}

	private void ToggleSound(ChangeEvent<bool> evt)
	{
		if (evt.newValue)
		{
			_buildingSoundController.EnableSound();
		}
		else
		{
			_buildingSoundController.DisableSound();
		}
	}
}
