using Timberborn.BaseComponentSystem;
using Timberborn.BehaviorSystem;
using Timberborn.BlockSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.EntitySystem;
using Timberborn.PrioritySystem;

namespace Timberborn.Demolishing;

internal class DemolishJob : BaseComponent, IAwakableComponent, IPostInitializableEntity
{
	private readonly DemolishJobs _demolishJobs;

	private Demolishable _demolishable;

	private BlockObject _blockObject;

	private BuilderPrioritizable _builderPrioritizable;

	private bool _canBeEnabled;

	private bool _enableAfterInitialization;

	private Priority Priority => _builderPrioritizable.Priority;

	public DemolishJob(DemolishJobs demolishJobs)
	{
		_demolishJobs = demolishJobs;
	}

	public void Awake()
	{
		_demolishable = GetComponent<Demolishable>();
		_blockObject = GetComponent<BlockObject>();
		_builderPrioritizable = GetComponent<BuilderPrioritizable>();
	}

	public void PostInitializeEntity()
	{
		_canBeEnabled = true;
		if (_enableAfterInitialization)
		{
			Enable();
		}
	}

	public void Enable()
	{
		if (_canBeEnabled)
		{
			_demolishJobs.AddJob(this, Priority);
			_builderPrioritizable.PriorityChanged += OnPriorityChanged;
		}
		else
		{
			_enableAfterInitialization = true;
		}
	}

	public void Disable()
	{
		_enableAfterInitialization = false;
		_demolishJobs.RemoveJob(this, Priority);
		_builderPrioritizable.PriorityChanged -= OnPriorityChanged;
	}

	public bool CanStartJob(Demolisher demolisher)
	{
		if (!_blockObject.CanDelete())
		{
			return false;
		}
		if (_demolishable.Reservable.Reserved || demolisher.HasReservedDemolishable)
		{
			return demolisher.IsReserved(_demolishable);
		}
		return true;
	}

	public (Behavior, Decision) StartBuilderJob(Demolisher demolisher)
	{
		if (!demolisher.IsReserved(_demolishable))
		{
			demolisher.Reserve(_demolishable);
		}
		DemolishBehavior component = demolisher.GetComponent<DemolishBehavior>();
		BehaviorAgent component2 = demolisher.GetComponent<BehaviorAgent>();
		return (component, component.Decide(component2));
	}

	private void OnPriorityChanged(object sender, PriorityChangedEventArgs priorityChangedEventArgs)
	{
		Priority previousPriority = priorityChangedEventArgs.PreviousPriority;
		_demolishJobs.RemoveJob(this, previousPriority);
		_demolishJobs.AddJob(this, Priority);
	}
}
