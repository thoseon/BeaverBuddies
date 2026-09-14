using Timberborn.BaseComponentSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.BuilderPrioritySystemUI;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.InventorySystemUI;
using Timberborn.PrioritySystemUI;
using Timberborn.RecoveredGoodSystem;
using UnityEngine.UIElements;

namespace Timberborn.RecoveredGoodSystemUI;

internal class RecoveredGoodStackFragment : IEntityPanelFragment
{
	private static readonly string PriorityLabelLocKey = "RecoveredGoodStack.Priority";

	private readonly BuilderPriorityToggleGroupFactory _builderPriorityToggleGroupFactory;

	private readonly InventoryFragmentBuilderFactory _inventoryFragmentBuilderFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private RecoveredGoodStack _recoveredGoodStack;

	private InventoryFragment _inventoryFragment;

	private VisualElement _root;

	private PriorityToggleGroup _priorityToggleGroup;

	public RecoveredGoodStackFragment(BuilderPriorityToggleGroupFactory builderPriorityToggleGroupFactory, InventoryFragmentBuilderFactory inventoryFragmentBuilderFactory, VisualElementLoader visualElementLoader)
	{
		_builderPriorityToggleGroupFactory = builderPriorityToggleGroupFactory;
		_inventoryFragmentBuilderFactory = inventoryFragmentBuilderFactory;
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/RecoveredGoodStackFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_priorityToggleGroup = _builderPriorityToggleGroupFactory.Create(_root, PriorityLabelLocKey);
		_inventoryFragment = _inventoryFragmentBuilderFactory.CreateBuilder(_root).Build();
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_recoveredGoodStack = entity.GetComponent<RecoveredGoodStack>();
		if ((bool)_recoveredGoodStack)
		{
			BuilderPrioritizable component = entity.GetComponent<BuilderPrioritizable>();
			_priorityToggleGroup.Enable(component);
			_inventoryFragment.ShowFragment(_recoveredGoodStack.Inventory);
		}
	}

	public void ClearFragment()
	{
		Hide();
		_recoveredGoodStack = null;
		_priorityToggleGroup.Disable();
		_inventoryFragment.ClearFragment();
	}

	public void UpdateFragment()
	{
		if ((bool)_recoveredGoodStack)
		{
			_root.ToggleDisplayStyle(visible: true);
			_inventoryFragment.UpdateFragment();
			_priorityToggleGroup.UpdateGroup();
		}
		else
		{
			Hide();
		}
	}

	private void Hide()
	{
		_root.ToggleDisplayStyle(visible: false);
	}
}
