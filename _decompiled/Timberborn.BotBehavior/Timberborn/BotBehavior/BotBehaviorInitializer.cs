using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.Carrying;
using Timberborn.CharacterControlSystem;
using Timberborn.DeathSystem;
using Timberborn.MortalSystem;
using Timberborn.NeedBehaviorSystem;
using Timberborn.Wandering;
using Timberborn.WorkSystem;

namespace Timberborn.BotBehavior;

internal class BotBehaviorInitializer : BaseComponent, IAwakableComponent
{
	public void Awake()
	{
		InitializeBehaviors();
	}

	private void InitializeBehaviors()
	{
		BehaviorManager component = GetComponent<BehaviorManager>();
		component.AddRootBehavior(GetComponent<CharacterControlRootBehavior>());
		component.AddRootBehavior(GetComponent<DeadRootBehavior>());
		component.AddRootBehavior(GetComponent<CarryRootBehavior>());
		component.AddRootBehavior(GetComponent<DieRootBehavior>());
		component.AddRootBehavior(GetComponent<CriticalNeederRootBehavior>());
		component.AddRootBehavior(GetComponent<StrandedRootBehavior>());
		component.AddRootBehavior(GetComponent<NeederRootBehavior>());
		component.AddRootBehavior(GetComponent<WorkerRootBehavior>());
		component.AddRootBehavior(GetComponent<WanderRootBehavior>());
	}
}
