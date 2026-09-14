using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using Timberborn.InputSystem;
using Timberborn.InputSystemUI;
using Timberborn.RecoveredGoodSystem;
using UnityEngine.UIElements;

namespace Timberborn.RecoveredGoodSystemUI;

internal class DeleteRecoveredGoodStackFragment : IEntityPanelFragment
{
	private static readonly string DeleteObjectKey = "DeleteObject";

	private static readonly string DeletePromptKey = "RecoveredGoodStack.DeletePrompt";

	private static readonly string SkipDeleteConfirmationKey = "SkipDeleteConfirmation";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly InputService _inputService;

	private readonly EntityService _entityService;

	private readonly BindableButtonFactory _bindableButtonFactory;

	private BindableButton _deleteButton;

	private VisualElement _root;

	private RecoveredGoodStack _recoveredGoodStack;

	public DeleteRecoveredGoodStackFragment(VisualElementLoader visualElementLoader, DialogBoxShower dialogBoxShower, InputService inputService, EntityService entityService, BindableButtonFactory bindableButtonFactory)
	{
		_visualElementLoader = visualElementLoader;
		_dialogBoxShower = dialogBoxShower;
		_inputService = inputService;
		_entityService = entityService;
		_bindableButtonFactory = bindableButtonFactory;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("Common/EntityPanel/DeleteObjectFragment");
		_deleteButton = _bindableButtonFactory.Create(_root.Q<Button>("Button"), DeleteObjectKey, DeleteCallback);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_recoveredGoodStack = entity.GetComponent<RecoveredGoodStack>();
		if ((bool)_recoveredGoodStack)
		{
			_root.ToggleDisplayStyle(visible: true);
			_deleteButton.Bind();
		}
	}

	public void ClearFragment()
	{
		if ((bool)_recoveredGoodStack)
		{
			_recoveredGoodStack = null;
			_deleteButton.Unbind();
		}
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
	}

	private void DeleteCallback()
	{
		if (_inputService.IsKeyHeld(SkipDeleteConfirmationKey))
		{
			DeleteRecoveredGoodStack();
		}
		else
		{
			ShowDialogBox();
		}
	}

	private void ShowDialogBox()
	{
		_dialogBoxShower.Create().SetLocalizedMessage(DeletePromptKey).SetConfirmButton(DeleteRecoveredGoodStack)
			.SetDefaultCancelButton()
			.Show();
	}

	private void DeleteRecoveredGoodStack()
	{
		if ((bool)_recoveredGoodStack)
		{
			_entityService.Delete(_recoveredGoodStack);
		}
	}
}
