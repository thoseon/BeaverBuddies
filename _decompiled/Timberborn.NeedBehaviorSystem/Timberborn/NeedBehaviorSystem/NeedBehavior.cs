using Timberborn.BehaviorSystem;
using Timberborn.EntitySystem;
using Timberborn.NeedSystem;
using UnityEngine;

namespace Timberborn.NeedBehaviorSystem;

public abstract class NeedBehavior : Behavior, IRegisteredComponent
{
	public abstract Vector3? ActionPosition(NeedManager needManager);
}
