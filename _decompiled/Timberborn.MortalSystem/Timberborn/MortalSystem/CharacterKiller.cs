using System.Collections.Generic;
using System.Linq;
using Timberborn.Characters;
using Timberborn.Common;
using Timberborn.Debugging;
using Timberborn.InputSystem;
using Timberborn.SelectionSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Timberborn.MortalSystem;

public class CharacterKiller : ILoadableSingleton, IInputProcessor, IDevModule
{
	private static readonly string DeleteObjectKey = "DeleteObject";

	private static readonly string SkipDeleteConfirmationKey = "SkipDeleteConfirmation";

	private static readonly float PartOfPopulation = 0.3f;

	private readonly EventBus _eventBus;

	private readonly InputService _inputService;

	private readonly DevModeManager _devModeManager;

	private readonly CharacterPopulation _characterPopulation;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	private Mortal _selectedMortal;

	public CharacterKiller(EventBus eventBus, InputService inputService, DevModeManager devModeManager, CharacterPopulation characterPopulation, IRandomNumberGenerator randomNumberGenerator)
	{
		_eventBus = eventBus;
		_inputService = inputService;
		_devModeManager = devModeManager;
		_characterPopulation = characterPopulation;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void Load()
	{
		_eventBus.Register(this);
		_inputService.AddInputProcessor(this);
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.CreateBindable("Kill selected character", DeleteObjectKey, delegate
		{
			KillSelectedCharacter(dieInstantly: false);
		})).AddMethod(DevMethod.Create($"Kill {PartOfPopulation * 100f:F0}% of characters", KillPartOfPopulation)).AddMethod(DevMethod.Create("Kill all characters instantly", KillAllPopulation))
			.AddMethod(DevMethod.Create("Kill all characters except selected", KillAllExceptSelected))
			.Build();
	}

	[OnEvent]
	public void OnSelectableObjectSelected(SelectableObjectSelectedEvent selectableObjectSelectedEvent)
	{
		Mortal component = selectableObjectSelectedEvent.SelectableObject.GetComponent<Mortal>();
		if ((bool)component)
		{
			_selectedMortal = component;
		}
	}

	[OnEvent]
	public void OnSelectableObjectUnselectedEvent(SelectableObjectUnselectedEvent selectableObjectUnselectedEvent)
	{
		_selectedMortal = null;
	}

	public bool ProcessInput()
	{
		if (_devModeManager.Enabled && (bool)_selectedMortal && _inputService.IsKeyDown(DeleteObjectKey))
		{
			KillSelectedCharacter(_inputService.IsKeyHeld(SkipDeleteConfirmationKey));
			return true;
		}
		return false;
	}

	private void KillSelectedCharacter(bool dieInstantly)
	{
		if ((bool)_selectedMortal && !_selectedMortal.ShouldDie)
		{
			KillCharacter(_selectedMortal, dieInstantly);
		}
	}

	private void KillPartOfPopulation()
	{
		List<Mortal> killableCharacters = GetKillableCharacters();
		if (!killableCharacters.IsEmpty())
		{
			int num = Mathf.CeilToInt(PartOfPopulation * (float)killableCharacters.Count);
			for (int i = 0; i < num; i++)
			{
				Mortal listElement = _randomNumberGenerator.GetListElement(killableCharacters);
				KillCharacter(listElement, dieInstantly: false);
				killableCharacters.Remove(listElement);
			}
		}
	}

	private void KillAllPopulation()
	{
		foreach (Mortal killableCharacter in GetKillableCharacters())
		{
			KillCharacter(killableCharacter, dieInstantly: true);
		}
	}

	private void KillAllExceptSelected()
	{
		foreach (Mortal killableCharacter in GetKillableCharacters())
		{
			if (killableCharacter != _selectedMortal)
			{
				KillCharacter(killableCharacter, dieInstantly: true);
			}
		}
	}

	private List<Mortal> GetKillableCharacters()
	{
		return (from character in _characterPopulation.Characters
			select character.GetComponent<Mortal>() into mortal
			where !mortal.ShouldDie
			select mortal).ToList();
	}

	private static void KillCharacter(Mortal mortal, bool dieInstantly)
	{
		string firstName = mortal.GetComponent<Character>().FirstName;
		if (dieInstantly)
		{
			mortal.DieInstantly(firstName + " was killed instantly by a heartless dev.");
		}
		else
		{
			mortal.DiePubliclyAsSoonAsPossible(firstName + " was forced to die by an evil dev.");
		}
	}
}
