using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DecalSystem;
using Timberborn.EntityPanelSystem;
using UnityEngine.UIElements;

namespace Timberborn.DecalSystemUI;

internal class FlippableDecalFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private VisualElement _root;

	private Toggle _toggle;

	private FlippableDecal _flippableDecal;

	public FlippableDecalFragment(VisualElementLoader visualElementLoader)
	{
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/FlippableDecalFragment");
		_root.ToggleDisplayStyle(visible: false);
		_toggle = _root.Q<Toggle>("Flip");
		_toggle.RegisterValueChangedCallback(delegate(ChangeEvent<bool> evt)
		{
			_flippableDecal.SetFlip(evt.newValue);
		});
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		FlippableDecal component = entity.GetComponent<FlippableDecal>();
		if (component != null)
		{
			_flippableDecal = component;
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_flippableDecal = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_flippableDecal)
		{
			_toggle.SetValueWithoutNotify(_flippableDecal.IsFlipped);
		}
	}
}
