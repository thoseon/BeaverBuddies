using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockSystemUI;
using Timberborn.Buildings;
using Timberborn.ConstructionSites;
using Timberborn.CoreUI;
using Timberborn.Debugging;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using Timberborn.InputSystem;
using Timberborn.InputSystemUI;
using Timberborn.RecoverableGoodSystemUI;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.BuildingsUI;

internal class DeleteBuildingFragment : IEntityPanelFragment
{
	private static readonly string DeleteObjectKey = "DeleteObject";

	private static readonly string SkipDeleteConfirmationKey = "SkipDeleteConfirmation";

	private static readonly string DeletePromptLocKey = "Buildings.DeletePrompt";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly RecoverableGoodDialogBoxShower _recoverableGoodDialogBoxShower;

	private readonly InputService _inputService;

	private readonly EntityService _entityService;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly DevModeManager _devModeManager;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private BlockObject _selectedBlockObject;

	private BlockObjectDeletionDescriber _deletionDescriber;

	private BindableButton _deleteButton;

	private VisualElement _root;

	public DeleteBuildingFragment(VisualElementLoader visualElementLoader, RecoverableGoodDialogBoxShower recoverableGoodDialogBoxShower, InputService inputService, EntityService entityService, BindableButtonFactory bindableButtonFactory, DevModeManager devModeManager, ITooltipRegistrar tooltipRegistrar)
	{
		_visualElementLoader = visualElementLoader;
		_recoverableGoodDialogBoxShower = recoverableGoodDialogBoxShower;
		_inputService = inputService;
		_entityService = entityService;
		_bindableButtonFactory = bindableButtonFactory;
		_devModeManager = devModeManager;
		_tooltipRegistrar = tooltipRegistrar;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/EntityPanel/DeleteObjectFragment");
		Button button = _root.Q<Button>("Button");
		_deleteButton = _bindableButtonFactory.Create(button, DeleteObjectKey, DeleteCallback);
		_tooltipRegistrar.RegisterWithKeyBinding(button, GetTooltipText, GetKeyBinding);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		Building component = entity.GetComponent<Building>();
		ConstructionSite component2 = entity.GetComponent<ConstructionSite>();
		BlockObject component3 = entity.GetComponent<BlockObject>();
		if ((bool)component || (bool)(BaseComponent)(object)component2 || (_devModeManager.Enabled && (bool)component3))
		{
			_selectedBlockObject = component3;
			_deletionDescriber = entity.GetComponent<BlockObjectDeletionDescriber>();
			_root.ToggleDisplayStyle(visible: true);
			_deleteButton.Bind();
		}
	}

	public void ClearFragment()
	{
		_selectedBlockObject = null;
		_deletionDescriber = null;
		_deleteButton.Unbind();
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_selectedBlockObject)
		{
			if (SelectedBuildingIsDeletable())
			{
				_deleteButton.Enable();
			}
			else
			{
				_deleteButton.Disable();
			}
		}
	}

	private void DeleteCallback()
	{
		if (_inputService.IsKeyHeld(SkipDeleteConfirmationKey))
		{
			DeleteBuilding();
		}
		else
		{
			ShowDialogBox();
		}
	}

	private string GetTooltipText()
	{
		return _deletionDescriber.GetDescription();
	}

	private string GetKeyBinding()
	{
		if (!SelectedBuildingIsDeletable())
		{
			return null;
		}
		return DeleteObjectKey;
	}

	private void ShowDialogBox()
	{
		if (SelectedBuildingIsDeletable())
		{
			_recoverableGoodDialogBoxShower.Show(_selectedBlockObject, DeleteBuilding, DeletePromptLocKey);
		}
	}

	private void DeleteBuilding()
	{
		if (SelectedBuildingIsDeletable())
		{
			_entityService.Delete(_selectedBlockObject);
		}
	}

	private bool SelectedBuildingIsDeletable()
	{
		if ((bool)_selectedBlockObject)
		{
			return _selectedBlockObject.CanDelete();
		}
		return false;
	}
}
