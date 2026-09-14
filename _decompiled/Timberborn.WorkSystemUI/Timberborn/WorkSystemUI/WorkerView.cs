using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CharactersUI;
using Timberborn.CoreUI;
using Timberborn.WorkSystem;
using Timberborn.WorkerTypesUI;
using UnityEngine.UIElements;

namespace Timberborn.WorkSystemUI;

internal class WorkerView
{
	private readonly WorkerTypeHelper _workerTypeHelper;

	private readonly CharacterButton _characterButton;

	private readonly Label _name;

	private readonly VisualElement _vacantIcon;

	private readonly WorkplaceWorkerType _workplaceWorkerType;

	public VisualElement Root { get; }

	public WorkerView(WorkerTypeHelper workerTypeHelper, VisualElement root, CharacterButton characterButton, Label name, VisualElement vacantIcon, WorkplaceWorkerType workplaceWorkerType)
	{
		_workerTypeHelper = workerTypeHelper;
		_characterButton = characterButton;
		Root = root;
		_name = name;
		_vacantIcon = vacantIcon;
		_workplaceWorkerType = workplaceWorkerType;
	}

	public void Fill(BaseComponent user, Action onClick, string description)
	{
		_characterButton.ShowFilled(user, onClick);
		_name.text = description;
		_vacantIcon.ToggleDisplayStyle(visible: false);
		Root.SetEnabled(value: true);
	}

	public void ShowEmpty()
	{
		ShowEmpty(vacant: false);
	}

	public void ShowVacant()
	{
		ShowEmpty(vacant: true);
	}

	private void ShowEmpty(bool vacant)
	{
		if (_workerTypeHelper.IsBotWorkerType(_workplaceWorkerType.WorkerType))
		{
			_characterButton.ShowBotEmpty();
		}
		else
		{
			_characterButton.ShowAdultEmpty();
		}
		_name.text = "";
		_vacantIcon.ToggleDisplayStyle(vacant);
		Root.SetEnabled(value: false);
	}
}
