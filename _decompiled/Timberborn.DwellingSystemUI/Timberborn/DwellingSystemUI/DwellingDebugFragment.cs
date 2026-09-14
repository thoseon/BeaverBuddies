using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.DwellingSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Reproduction;
using UnityEngine.UIElements;

namespace Timberborn.DwellingSystemUI;

internal class DwellingDebugFragment : IEntityPanelFragment
{
	private readonly DebugFragmentFactory _debugFragmentFactory;

	private readonly NewbornSpawner _newbornSpawner;

	private Dwelling _dwelling;

	private VisualElement _root;

	private Button _button;

	public DwellingDebugFragment(DebugFragmentFactory debugFragmentFactory, NewbornSpawner newbornSpawner)
	{
		_debugFragmentFactory = debugFragmentFactory;
		_newbornSpawner = newbornSpawner;
	}

	public VisualElement InitializeFragment()
	{
		DebugFragmentButton debugFragmentButton = new DebugFragmentButton(SpawnNewborn, "Spawn newborn");
		_root = _debugFragmentFactory.Create(debugFragmentButton);
		_button = _root.Q<Button>("Button");
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_dwelling = entity.GetComponent<Dwelling>();
	}

	public void ClearFragment()
	{
		_dwelling = null;
	}

	public void UpdateFragment()
	{
		if ((bool)_dwelling && _dwelling.Enabled)
		{
			_root.ToggleDisplayStyle(visible: true);
			_button.SetEnabled(_dwelling.HasFreeSlots);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	private void SpawnNewborn()
	{
		if ((bool)_dwelling && _dwelling.Enabled && _dwelling.HasFreeSlots)
		{
			_newbornSpawner.SpawnChild(_dwelling);
		}
	}
}
