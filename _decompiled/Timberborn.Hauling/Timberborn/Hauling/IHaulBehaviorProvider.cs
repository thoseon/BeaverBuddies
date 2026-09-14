using System.Collections.Generic;

namespace Timberborn.Hauling;

public interface IHaulBehaviorProvider
{
	void GetWeightedBehaviors(IList<WeightedBehavior> weightedBehaviors);
}
