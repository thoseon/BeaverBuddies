using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BlockSystemUI;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using Timberborn.InputSystemUI;
using Timberborn.TooltipSystem;
using Timberborn.UndoSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorUI;

internal class DeleteBlockObjectFragment : IEntityPanelFragment
{
	private static readonly string DeleteObjectKey = "DeleteObject";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly EntityService _entityService;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly IUndoRegistry _undoRegistry;

	private BlockObject _selectedBlockObject;

	private BlockObjectDeletionDescriber _deletionDescriber;

	private VisualElement _root;

	private BindableButton _deleteButton;

	public DeleteBlockObjectFragment(VisualElementLoader visualElementLoader, EntityService entityService, BindableButtonFactory bindableButtonFactory, ITooltipRegistrar tooltipRegistrar, IUndoRegistry undoRegistry)
	{
		_visualElementLoader = visualElementLoader;
		_entityService = entityService;
		_bindableButtonFactory = bindableButtonFactory;
		_tooltipRegistrar = tooltipRegistrar;
		_undoRegistry = undoRegistry;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/EntityPanel/DeleteObjectFragment");
		Button button = _root.Q<Button>("Button");
		_deleteButton = _bindableButtonFactory.Create(button, DeleteObjectKey, DeleteObject);
		_tooltipRegistrar.RegisterWithKeyBinding(button, GetTooltipText, GetKeyBinding);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_selectedBlockObject = entity.GetComponent<BlockObject>();
		if ((bool)_selectedBlockObject)
		{
			_deletionDescriber = _selectedBlockObject.GetComponent<BlockObjectDeletionDescriber>();
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
		if (IsSelectedObjectDeletable())
		{
			_deleteButton.Enable();
		}
		else
		{
			_deleteButton.Disable();
		}
	}

	private void DeleteObject()
	{
		if (IsSelectedObjectDeletable())
		{
			_entityService.Delete(_selectedBlockObject);
			_undoRegistry.CommitStack();
		}
	}

	private string GetTooltipText()
	{
		return _deletionDescriber.GetDescription();
	}

	private string GetKeyBinding()
	{
		if (!IsSelectedObjectDeletable())
		{
			return null;
		}
		return DeleteObjectKey;
	}

	private bool IsSelectedObjectDeletable()
	{
		if ((bool)_selectedBlockObject)
		{
			return _selectedBlockObject.CanDelete();
		}
		return false;
	}
}
