using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CharactersUI;
using Timberborn.CoreUI;
using UnityEngine.UIElements;

namespace Timberborn.DwellingSystemUI;

internal class DwellerView
{
	private static readonly string NegativeWellbeingClass = "wellbeing--negative";

	private readonly CharacterButton _characterButton;

	private readonly Button _viewButton;

	private readonly Label _name;

	private readonly Label _subtitle;

	private readonly Label _wellbeingCounter;

	private bool _isChildSlot;

	public VisualElement Root { get; }

	public DwellerView(VisualElement root, CharacterButton characterButton, Button viewButton, Label name, Label subtitle, Label wellbeingCounter)
	{
		Root = root;
		_characterButton = characterButton;
		_viewButton = viewButton;
		_name = name;
		_subtitle = subtitle;
		_wellbeingCounter = wellbeingCounter;
	}

	public void SetAsAdult()
	{
		_characterButton.ShowAdultEmpty();
		_isChildSlot = false;
		Clear();
	}

	public void SetAsChild()
	{
		_characterButton.ShowChildEmpty();
		_isChildSlot = true;
		Clear();
	}

	public void Fill(BaseComponent user, Action onClick, string name, string subtitle, int wellbeing)
	{
		_characterButton.ShowFilled(user, onClick);
		_name.text = name;
		_subtitle.text = subtitle;
		_wellbeingCounter.ToggleDisplayStyle(visible: true);
		_wellbeingCounter.text = wellbeing.ToString();
		_wellbeingCounter.EnableInClassList(NegativeWellbeingClass, wellbeing < 0);
		_viewButton.SetEnabled(value: true);
	}

	public void Reset()
	{
		if (_isChildSlot)
		{
			SetAsChild();
		}
		else
		{
			SetAsAdult();
		}
	}

	private void Clear()
	{
		_name.text = "";
		_subtitle.text = "";
		_wellbeingCounter.ToggleDisplayStyle(visible: false);
		_wellbeingCounter.RemoveFromClassList(NegativeWellbeingClass);
		_viewButton.SetEnabled(value: false);
	}
}
