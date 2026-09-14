using Timberborn.BlueprintSystem;
using Timberborn.SingletonSystem;

namespace Timberborn.TickSystem;

internal class TickService : ITickService, ILoadableSingleton
{
	private readonly ISpecService _specService;

	public float TickIntervalInSeconds { get; private set; }

	public TickService(ISpecService specService)
	{
		_specService = specService;
	}

	public void Load()
	{
		TickTimeSpec singleSpec = _specService.GetSingleSpec<TickTimeSpec>();
		TickIntervalInSeconds = singleSpec.TickIntervalInSeconds;
	}
}
