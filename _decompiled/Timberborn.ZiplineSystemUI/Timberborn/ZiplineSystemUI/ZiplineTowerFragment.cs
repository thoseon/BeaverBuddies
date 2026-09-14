using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.ZiplineSystem;
using UnityEngine.UIElements;

namespace Timberborn.ZiplineSystemUI;

internal class ZiplineTowerFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ZiplineConnectionButtonFactory _ziplineConnectionButtonFactory;

	private VisualElement _root;

	private VisualElement _buttons;

	private ZiplineTower _ziplineTower;

	public ZiplineTowerFragment(VisualElementLoader visualElementLoader, ZiplineConnectionButtonFactory ziplineConnectionButtonFactory)
	{
		_visualElementLoader = visualElementLoader;
		_ziplineConnectionButtonFactory = ziplineConnectionButtonFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/ZiplineTowerFragment");
		_buttons = _root.Q<VisualElement>("Buttons");
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_ziplineTower = entity.GetComponent<ZiplineTower>();
		if ((bool)_ziplineTower)
		{
			CreateButtons();
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void UpdateFragment()
	{
	}

	public void ClearFragment()
	{
		_ziplineTower = null;
		_buttons.Clear();
		_root.ToggleDisplayStyle(visible: false);
	}

	private void CreateButtons()
	{
		int i;
		for (i = 0; i < _ziplineTower.ConnectionTargets.Count; i++)
		{
			ZiplineTower otherZiplineTower = _ziplineTower.ConnectionTargets[i];
			_ziplineConnectionButtonFactory.CreateConnection(_buttons, _ziplineTower, otherZiplineTower);
		}
		if (_ziplineTower.HasFreeSlots)
		{
			CreateFreeSlotButtons(++i);
		}
	}

	private void CreateFreeSlotButtons(int index)
	{
		_ziplineConnectionButtonFactory.CreateAddConnection(_buttons, _ziplineTower);
		while (index < _ziplineTower.MaxConnections)
		{
			_ziplineConnectionButtonFactory.CreateEmpty(_buttons);
			index++;
		}
	}
}
