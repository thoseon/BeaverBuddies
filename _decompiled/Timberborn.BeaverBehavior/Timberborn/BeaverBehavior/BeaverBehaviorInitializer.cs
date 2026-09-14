using Timberborn.BaseComponentSystem;
using Timberborn.BeaverContaminationSystem;
using Timberborn.Beavers;
using Timberborn.BehaviorSystem;
using Timberborn.Carrying;
using Timberborn.CharacterControlSystem;
using Timberborn.DeathSystem;
using Timberborn.MortalSystem;
using Timberborn.NeedBehaviorSystem;
using Timberborn.SleepSystem;
using Timberborn.Wandering;
using Timberborn.WorkSystem;

namespace Timberborn.BeaverBehavior;

internal class BeaverBehaviorInitializer : BaseComponent, IAwakableComponent
{
	public void Awake()
	{
		bool isAdult = !(BaseComponent)(object)GetComponent<Child>();
		InitializeBehaviors(isAdult);
	}

	private void InitializeBehaviors(bool isAdult)
	{
		BehaviorManager component = GetComponent<BehaviorManager>();
		BeaverNeedBehaviorPicker component2 = GetComponent<BeaverNeedBehaviorPicker>();
		component.AddRootBehavior(GetComponent<CharacterControlRootBehavior>());
		component.AddRootBehavior(GetComponent<DeadRootBehavior>());
		if (isAdult)
		{
			component.AddRootBehavior(GetComponent<CarryRootBehavior>());
		}
		else
		{
			component.AddRootBehavior(GetComponent<ChildRootBehavior>());
		}
		component.AddRootBehavior(GetComponent<DieRootBehavior>());
		component.AddRootBehavior(GetComponent<ContaminateRootBehavior>());
		component.AddRootBehavior(GetComponent<CriticalNeederRootBehavior>());
		component.AddRootBehavior(GetComponent<StrandedRootBehavior>());
		if (isAdult)
		{
			component.AddRootBehavior(GetComponent<WorkerRootBehavior>());
		}
		component.AddRootBehavior(GetComponent<NeederRootBehavior>());
		component2.InitializeEssentialNeedBehavior(GetComponent<SleepNeedBehavior>());
		WanderRootBehavior component3 = GetComponent<WanderRootBehavior>();
		component.AddRootBehavior(component3);
		component3.AllowVisitingRestPlaces();
	}
}
