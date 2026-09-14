using System;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.RecoverableGoodSystemUI;

public class RecoverableGoodDialogBoxShower
{
	private readonly DialogBoxShower _dialogBoxShower;

	private readonly RecoverableGoodElementFactory _recoverableGoodElementFactory;

	public RecoverableGoodDialogBoxShower(DialogBoxShower dialogBoxShower, RecoverableGoodElementFactory recoverableGoodElementFactory)
	{
		_dialogBoxShower = dialogBoxShower;
		_recoverableGoodElementFactory = recoverableGoodElementFactory;
	}

	public void Show(BlockObject blockObject, Action confirmAction, string promptLocKey)
	{
		VisualElement content = _recoverableGoodElementFactory.Create(Enumerables.One(blockObject));
		_dialogBoxShower.Create().SetLocalizedMessage(promptLocKey).SetConfirmButton(confirmAction)
			.SetDefaultCancelButton()
			.AddContent(content)
			.Show();
	}
}
