using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.Hauling;

namespace Timberborn.Emptying;

public class EmptiableHaulBehaviorProvider : BaseComponent, IAwakableComponent, IHaulBehaviorProvider
{
	private static readonly float EmptiableWeight = 0.51f;

	private Emptiable _emptiable;

	private EmptyInventoriesWorkplaceBehavior _emptyInventoriesWorkplaceBehavior;

	public void Awake()
	{
		_emptiable = GetComponent<Emptiable>();
		_emptyInventoriesWorkplaceBehavior = GetComponent<EmptyInventoriesWorkplaceBehavior>();
	}

	public void GetWeightedBehaviors(IList<WeightedBehavior> weightedBehaviors)
	{
		if (_emptiable.IsMarkedForEmptying)
		{
			weightedBehaviors.Add(new WeightedBehavior(EmptiableWeight, _emptyInventoriesWorkplaceBehavior));
		}
	}
}
