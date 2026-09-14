using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using UnityEngine;

namespace Timberborn.Characters;

internal class GameSpeedThrottler : IPostLoadableSingleton
{
	private readonly CharacterPopulation _characterPopulation;

	private readonly EventBus _eventBus;

	private readonly SpeedManager _speedManager;

	private readonly ISpecService _specService;

	private GameSpeedThrottlerSpec _spec;

	public GameSpeedThrottler(CharacterPopulation characterPopulation, EventBus eventBus, SpeedManager speedManager, ISpecService specService)
	{
		_characterPopulation = characterPopulation;
		_eventBus = eventBus;
		_speedManager = speedManager;
		_specService = specService;
	}

	public void PostLoad()
	{
		_spec = _specService.GetSingleSpec<GameSpeedThrottlerSpec>();
		_eventBus.Register(this);
		ThrottleGameSpeed();
	}

	[OnEvent]
	public void OnCharacterCreated(CharacterCreatedEvent characterCreatedEvent)
	{
		ThrottleGameSpeed();
	}

	[OnEvent]
	public void OnCharacterKilled(CharacterKilledEvent characterKilledEvent)
	{
		ThrottleGameSpeed();
	}

	private void ThrottleGameSpeed()
	{
		float speedScale = PopulationToSpeedScale(_characterPopulation.NumberOfCharacters);
		_speedManager.ChangeSpeedScale(speedScale);
	}

	private float PopulationToSpeedScale(int population)
	{
		int num = Mathf.Clamp(population, _spec.MinPopulation, _spec.MaxPopulation);
		float num2 = Mathf.InverseLerp(_spec.MinPopulation, _spec.MaxPopulation, num);
		return Mathf.Lerp(_spec.MinGameSpeedScale, _spec.MaxGameSpeedScale, 1f - num2);
	}
}
