using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;

namespace Timberborn.BeaverContaminationSystem;

public class ContaminateRootBehavior : RootBehavior, IAwakableComponent
{
	private Contaminable _contaminable;

	private ContaminationIncubator _contaminationIncubator;

	public void Awake()
	{
		_contaminable = GetComponent<Contaminable>();
		_contaminationIncubator = GetComponent<ContaminationIncubator>();
	}

	public override Decision Decide(BehaviorAgent agent)
	{
		if (_contaminationIncubator.IncubationFinished && !_contaminable.IsContaminated)
		{
			_contaminable.Contaminate();
			_contaminationIncubator.ResetIncubation();
			return Decision.ReleaseNextTick();
		}
		return Decision.ReleaseNow();
	}
}
