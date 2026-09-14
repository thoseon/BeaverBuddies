using System.Collections.Generic;
using System.Linq;
using System.Text;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.KeyBindingSystem;
using Timberborn.Localization;

namespace Timberborn.KeyBindingSystemUI;

public class KeyRebinder
{
	private static readonly string RebindingMessageLocKey = "KeyBindingBox.RebindingMessage";

	private static readonly string ClearBindingLocKey = "KeyBindingBox.ClearBinding";

	private static readonly string DuplicatedBindingLocKey = "KeyBindingBox.DuplicatedBinding";

	private static readonly string MaxBindingCollisionLine = "...";

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly InputBindingListener _inputBindingListener;

	private readonly KeyBindingRegistry _keyBindingRegistry;

	private readonly KeyBindingSpecService _keyBindingSpecService;

	private readonly ILoc _loc;

	private DefinableInputBinding _definableInputBinding;

	private DialogBox _dialogBox;

	public KeyRebinder(DialogBoxShower dialogBoxShower, InputBindingListener inputBindingListener, KeyBindingRegistry keyBindingRegistry, KeyBindingSpecService keyBindingSpecService, ILoc loc)
	{
		_dialogBoxShower = dialogBoxShower;
		_inputBindingListener = inputBindingListener;
		_keyBindingRegistry = keyBindingRegistry;
		_keyBindingSpecService = keyBindingSpecService;
		_loc = loc;
	}

	public void StartRebinding(DefinableInputBinding singleInputBinding)
	{
		Asserts.FieldIsNull(this, _definableInputBinding, "_definableInputBinding");
		_definableInputBinding = singleInputBinding;
		ShowRebinder();
	}

	private void ShowRebinder()
	{
		StartListeningForInput();
		_dialogBox = _dialogBoxShower.Create().SetMessage(_loc.T(RebindingMessageLocKey, _definableInputBinding.KeyBinding.DisplayName)).SetConfirmButton(ClearBinding, _loc.T(ClearBindingLocKey))
			.SetCancelButton(Cancel, _loc.T(CommonLocKeys.CancelKey))
			.Show();
	}

	private void StartListeningForInput()
	{
		_inputBindingListener.WaitForInput(InputCallback);
	}

	private void InputCallback(CustomInputBinding customInputBinding)
	{
		List<KeyBinding> list = CollidingBindings(customInputBinding).ToList();
		if (list.Count > 0)
		{
			ShowCollidingBindingsDialog(list, customInputBinding);
		}
		else
		{
			RebindAndClose(customInputBinding);
		}
	}

	private IEnumerable<KeyBinding> CollidingBindings(CustomInputBinding customInputBinding)
	{
		foreach (KeyBinding keyBinding in _keyBindingRegistry.KeyBindings)
		{
			if (keyBinding.IsUsingBinding(customInputBinding))
			{
				keyBinding.Lock();
				if (keyBinding != _definableInputBinding.KeyBinding)
				{
					yield return keyBinding;
				}
			}
		}
	}

	private void ShowCollidingBindingsDialog(IReadOnlyList<KeyBinding> collidingBindings, CustomInputBinding customInputBinding)
	{
		_dialogBoxShower.Create().SetMessage(GetCollidingBindingsMessage(collidingBindings)).SetConfirmButton(delegate
		{
			RebindAndClose(customInputBinding);
		})
			.SetCancelButton(StartListeningForInput)
			.Show();
	}

	private string GetCollidingBindingsMessage(IReadOnlyList<KeyBinding> collidingBindings)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < collidingBindings.Count; i++)
		{
			if (i >= 20)
			{
				stringBuilder.AppendLine(MaxBindingCollisionLine);
				break;
			}
			stringBuilder.AppendLine(SpecialStrings.RowStarter + collidingBindings[i].DisplayName);
		}
		return _loc.T(DuplicatedBindingLocKey, stringBuilder.ToStringWithoutNewLineEnd());
	}

	private void RebindAndClose(CustomInputBinding customInputBinding)
	{
		RebindKey(customInputBinding);
		_dialogBox.Close();
		ClearRebinder();
	}

	private void RebindKey(CustomInputBinding customInputBinding)
	{
		_keyBindingSpecService.RebindInputBinding(_definableInputBinding, customInputBinding);
	}

	private void ClearRebinder()
	{
		_dialogBox = null;
		_definableInputBinding = null;
	}

	private void ClearBinding()
	{
		RebindKey(CustomInputBinding.UndefinedBinding);
		Cancel();
	}

	private void Cancel()
	{
		_inputBindingListener.FinishListening();
		ClearRebinder();
	}
}
