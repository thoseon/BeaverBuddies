using System;
using Timberborn.CoreUI;
using Timberborn.FileSystem;
using Timberborn.GameSaveRepositorySystem;
using UnityEngine.UIElements;

namespace Timberborn.SettlementNameSystemUI;

internal class SettlementNameBox : IPanelController
{
	private static readonly string InvalidNameLocKey = "Saving.InvalidName";

	private static readonly string TakenNameLocKey = "Saving.TakenName";

	private readonly PanelStack _panelStack;

	private readonly GameSaveRepository _gameSaveRepository;

	private readonly DialogBoxShower _dialogBoxShower;

	private readonly Action<string> _confirmButtonCallback;

	private readonly VisualElement _root;

	private readonly TextField _input;

	private readonly string _initialSettlementName;

	public string SettlementName => _input.text;

	public SettlementNameBox(PanelStack panelStack, GameSaveRepository gameSaveRepository, DialogBoxShower dialogBoxShower, Action<string> confirmButtonCallback, VisualElement root, string initialSettlementName)
	{
		_panelStack = panelStack;
		_gameSaveRepository = gameSaveRepository;
		_dialogBoxShower = dialogBoxShower;
		_confirmButtonCallback = confirmButtonCallback;
		_root = root;
		_input = _root.Q<TextField>("Input");
		_initialSettlementName = initialSettlementName;
	}

	public VisualElement GetPanel()
	{
		_input.SetValueWithoutNotify(_initialSettlementName);
		return _root;
	}

	public bool OnUIConfirmed()
	{
		string text = _input.text;
		if (!string.IsNullOrEmpty(text))
		{
			switch (_gameSaveRepository.CreateDirectoryForSettlement(text))
			{
			case DirectoryCreationResult.OK:
				_panelStack.Pop(this);
				_confirmButtonCallback(text);
				break;
			case DirectoryCreationResult.NameTaken:
				ShowDialogBox(TakenNameLocKey);
				break;
			case DirectoryCreationResult.NameInvalid:
				ShowDialogBox(InvalidNameLocKey);
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		return true;
	}

	public void OnUICancelled()
	{
	}

	private void ShowDialogBox(string textLocKey)
	{
		_dialogBoxShower.Create().SetLocalizedMessage(textLocKey).Show();
	}
}
