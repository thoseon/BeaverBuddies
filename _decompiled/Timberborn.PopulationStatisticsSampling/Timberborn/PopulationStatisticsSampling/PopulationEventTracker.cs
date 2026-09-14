using Timberborn.Bots;
using Timberborn.CharactersGame;
using Timberborn.GameDistricts;
using Timberborn.MortalSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.PopulationStatisticsSampling;

internal class PopulationEventTracker : IPostLoadableSingleton
{
	private readonly EventBus _eventBus;

	public PopulationEventTracker(EventBus eventBus)
	{
		_eventBus = eventBus;
	}

	public void PostLoad()
	{
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnCharacterBirth(CharacterBirthEvent characterBirthEvent)
	{
		Citizen component = characterBirthEvent.CharacterBirth.GetComponent<Citizen>();
		DistrictPopulationBalance districtPopulationBalance = component.AssignedDistrict?.GetComponent<DistrictPopulationBalance>();
		if (districtPopulationBalance != null && (bool)districtPopulationBalance)
		{
			if (component.HasComponent<BotSpec>())
			{
				districtPopulationBalance.BotCreations++;
			}
			else
			{
				districtPopulationBalance.Births++;
			}
		}
	}

	[OnEvent]
	public void OnPreMortalDiedEvent(PreMortalDiedEvent preMortalDiedEvent)
	{
		Citizen component = preMortalDiedEvent.Mortal.GetComponent<Citizen>();
		DistrictPopulationBalance districtPopulationBalance = component.AssignedDistrict?.GetComponent<DistrictPopulationBalance>();
		if (districtPopulationBalance != null && (bool)districtPopulationBalance)
		{
			if (component.HasComponent<BotSpec>())
			{
				districtPopulationBalance.BotDestructions++;
			}
			else
			{
				districtPopulationBalance.Deaths++;
			}
		}
	}
}
