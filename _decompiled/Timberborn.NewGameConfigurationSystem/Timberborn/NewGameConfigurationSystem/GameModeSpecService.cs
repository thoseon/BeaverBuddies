using System.Collections.Immutable;
using System.Linq;
using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.NewGameConfigurationSystem;

public class GameModeSpecService : ILoadableSingleton
{
	private readonly ISpecService _specService;

	private ImmutableArray<GameModeSpec> _gameModeSpecsOrdered;

	private GameModeSpec _defaultGameModeSpec;

	public GameModeSpecService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		_gameModeSpecsOrdered = (from spec in _specService.GetSpecs<GameModeSpec>()
			orderby spec.Order
			select spec).ToImmutableArray();
		_defaultGameModeSpec = _gameModeSpecsOrdered.First((GameModeSpec spec) => spec.IsDefault);
	}

	public ImmutableArray<GameModeSpec> GetSpecsOrdered()
	{
		return _gameModeSpecsOrdered;
	}

	public GameModeSpec GetDefaultSpec()
	{
		return _defaultGameModeSpec;
	}
}
