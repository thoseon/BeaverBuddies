using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.GameSound;
using Timberborn.InputSystem;
using Timberborn.InventorySystemUI;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using Timberborn.Wonders;
using UnityEngine.UIElements;

namespace Timberborn.WondersUI;

internal class WonderFragment : IEntityPanelFragment, IInputProcessor
{
	private static readonly string WonderActivateLocKey = "Wonder.Activate";

	private static readonly string UniqueBuildingActionKey = "UniqueBuildingAction";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly InventoryFragmentBuilderFactory _inventoryFragmentBuilderFactory;

	private readonly GameUISoundController _gameUISoundController;

	private readonly InputService _inputService;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	private VisualElement _root;

	private Wonder _wonder;

	private WonderInventory _wonderInventory;

	private InventoryFragment _inventoryFragment;

	private Button _button;

	public WonderFragment(VisualElementLoader visualElementLoader, InventoryFragmentBuilderFactory inventoryFragmentBuilderFactory, GameUISoundController gameUISoundController, InputService inputService, ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_inventoryFragmentBuilderFactory = inventoryFragmentBuilderFactory;
		_gameUISoundController = gameUISoundController;
		_inputService = inputService;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/EntityPanel/WonderFragment");
		VisualElement root = _root.Q<VisualElement>("InventoryRoot");
		_inventoryFragment = _inventoryFragmentBuilderFactory.CreateBuilder(root).ShowEmptyRows().ShowRowLimit()
			.Build();
		_root.ToggleDisplayStyle(visible: false);
		_button = _root.Q<Button>("ActivateButton");
		_button.RegisterCallback<ClickEvent>(delegate
		{
			ActivateWonder();
		});
		_tooltipRegistrar.RegisterWithKeyBinding(_button, _loc.T(WonderActivateLocKey), UniqueBuildingActionKey);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_wonder = entity.GetComponent<Wonder>();
		_wonderInventory = entity.GetComponent<WonderInventory>();
		if ((bool)_wonderInventory)
		{
			_inventoryFragment.ShowFragment(_wonderInventory.Inventory);
			_inputService.AddInputProcessor(this);
		}
	}

	public void UpdateFragment()
	{
		if ((bool)_wonder && _wonder.Enabled)
		{
			_button.SetEnabled(_wonder.CanBeActivated());
			_inventoryFragment.UpdateFragment();
			_root.ToggleDisplayStyle(visible: true);
		}
		else
		{
			_root.ToggleDisplayStyle(visible: false);
		}
	}

	public void ClearFragment()
	{
		_wonder = null;
		_root.ToggleDisplayStyle(visible: false);
		_inventoryFragment.ClearFragment();
		_inputService.RemoveInputProcessor(this);
	}

	public bool ProcessInput()
	{
		if (_inputService.IsKeyDown(UniqueBuildingActionKey))
		{
			ActivateWonder();
			return true;
		}
		return false;
	}

	private void ActivateWonder()
	{
		_wonder.Activate();
		_gameUISoundController.PlayWonderLaunchSound();
	}
}
