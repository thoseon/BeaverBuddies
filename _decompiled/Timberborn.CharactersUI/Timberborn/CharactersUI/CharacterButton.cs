using System;
using Timberborn.BaseComponentSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.GameFactionSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.CharactersUI;

public class CharacterButton
{
	private readonly FactionService _factionService;

	private readonly EntityBadgeService _entityBadgeService;

	private Button _button;

	public VisualElement Root { get; }

	public Action ClickAction { get; private set; }

	public CharacterButton(VisualElement root, FactionService factionService, EntityBadgeService entityBadgeService)
	{
		Root = root;
		_factionService = factionService;
		_entityBadgeService = entityBadgeService;
	}

	public void Initialize()
	{
		_button = Root.Q<Button>("CharacterButton");
		_button.RegisterCallback<ClickEvent>(InvokeClickAction);
	}

	public void ShowFilled(BaseComponent user, Action onClick)
	{
		Sprite entityAvatar = _entityBadgeService.GetEntityAvatar(user);
		ShowFilled(entityAvatar, onClick);
	}

	public void ShowAdultEmpty()
	{
		SetBackground(_factionService.Current.Avatar.Asset);
		ChangeOnClickAction();
	}

	public void ShowChildEmpty()
	{
		SetBackground(_factionService.Current.ChildAvatar.Asset);
		ChangeOnClickAction();
	}

	public void ShowBotEmpty()
	{
		SetBackground(_factionService.Current.BotAvatar.Asset);
		ChangeOnClickAction();
	}

	public void ShowEmpty()
	{
		_button.style.backgroundImage = null;
		ChangeOnClickAction();
	}

	private void ShowFilled(Sprite avatar, Action onClick)
	{
		_button.style.backgroundImage = null;
		SetBackground(avatar);
		ChangeOnClickAction(onClick);
	}

	private void SetBackground(Sprite sprite)
	{
		_button.style.backgroundImage = new StyleBackground(sprite);
	}

	private void ChangeOnClickAction(Action onClick = null)
	{
		_button.SetEnabled(onClick != null);
		ClickAction = onClick;
	}

	private void InvokeClickAction(ClickEvent evt)
	{
		ClickAction?.Invoke();
	}
}
