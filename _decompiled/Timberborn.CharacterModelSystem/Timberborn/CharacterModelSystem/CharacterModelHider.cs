using System.Collections.Generic;
using Timberborn.EntitySystem;
using Timberborn.LevelVisibilitySystem;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using UnityEngine;

namespace Timberborn.CharacterModelSystem;

internal class CharacterModelHider : ILoadableSingleton, ITickableSingleton
{
	private readonly EventBus _eventBus;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly List<CharacterModel> _characters = new List<CharacterModel>();

	public CharacterModelHider(EventBus eventBus, ILevelVisibilityService levelVisibilityService)
	{
		_eventBus = eventBus;
		_levelVisibilityService = levelVisibilityService;
	}

	public void Load()
	{
		_eventBus.Register(this);
	}

	public void Tick()
	{
		if (!_levelVisibilityService.LevelIsAtMax)
		{
			UpdateVisibilityOfModels();
		}
	}

	[OnEvent]
	public void OnEntityInitialized(EntityInitializedEvent entityInitializedEvent)
	{
		CharacterModel component = entityInitializedEvent.Entity.GetComponent<CharacterModel>();
		if ((bool)component)
		{
			_characters.Add(component);
		}
	}

	[OnEvent]
	public void OnEntityDeleted(EntityDeletedEvent entityDeletedEvent)
	{
		CharacterModel component = entityDeletedEvent.Entity.GetComponent<CharacterModel>();
		if ((bool)component)
		{
			_characters.Remove(component);
		}
	}

	[OnEvent]
	public void OnMaxVisibleLevelChanged(MaxVisibleLevelChangedEvent maxVisibleLevelChangedEvent)
	{
		UpdateVisibilityOfModels();
	}

	private void UpdateVisibilityOfModels()
	{
		foreach (CharacterModel character in _characters)
		{
			Vector3Int coordinates = NavigationCoordinateSystem.WorldToGridInt(character.Transform.position);
			if (_levelVisibilityService.BlockIsVisible(coordinates))
			{
				character.UnblockModel();
			}
			else
			{
				character.BlockModel();
			}
		}
	}
}
