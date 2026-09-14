using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.ScienceSystem;

namespace Timberborn.Demolishing;

public class DemolishableScienceReward : BaseComponent, IAwakableComponent, IDeletableEntity
{
	private readonly ScienceService _scienceService;

	private DemolishableScienceRewardSpec _demolishableScienceRewardSpec;

	private Demolishable _demolishable;

	public DemolishableScienceReward(ScienceService scienceService)
	{
		_scienceService = scienceService;
	}

	public void Awake()
	{
		_demolishableScienceRewardSpec = GetComponent<DemolishableScienceRewardSpec>();
		_demolishable = GetComponent<Demolishable>();
	}

	public void DeleteEntity()
	{
		if (_demolishable.DemolishingProgress >= 1f)
		{
			_scienceService.AddPoints(_demolishableScienceRewardSpec.SciencePoints);
		}
	}
}
