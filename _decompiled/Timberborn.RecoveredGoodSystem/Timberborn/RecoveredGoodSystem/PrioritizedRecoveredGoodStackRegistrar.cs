using Timberborn.BaseComponentSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.EntitySystem;
using Timberborn.PrioritySystem;
using Timberborn.TemplateSystem;

namespace Timberborn.RecoveredGoodSystem;

internal class PrioritizedRecoveredGoodStackRegistrar : BaseComponent, IAwakableComponent, IInitializableEntity, IDeletableEntity
{
	private readonly PrioritizedRecoveredGoodStackRegistry _prioritizedRecoveredGoodStackRegistry;

	private RecoveredGoodStack _recoveredGoodStack;

	private InstantiatedTemplate _instantiatedTemplate;

	private BuilderPrioritizable _builderPrioritizable;

	private Priority PrioritizablePriority => _builderPrioritizable.Priority;

	private int InstantiationOrder => _instantiatedTemplate.InstantiationOrder;

	public PrioritizedRecoveredGoodStackRegistrar(PrioritizedRecoveredGoodStackRegistry prioritizedRecoveredGoodStackRegistry)
	{
		_prioritizedRecoveredGoodStackRegistry = prioritizedRecoveredGoodStackRegistry;
	}

	public void Awake()
	{
		_recoveredGoodStack = GetComponent<RecoveredGoodStack>();
		_instantiatedTemplate = GetComponent<InstantiatedTemplate>();
		_builderPrioritizable = GetComponent<BuilderPrioritizable>();
	}

	public void InitializeEntity()
	{
		_builderPrioritizable.Enable();
		_builderPrioritizable.PriorityChanged += OnPriorityChanged;
		_prioritizedRecoveredGoodStackRegistry.AddStack(_recoveredGoodStack, PrioritizablePriority, InstantiationOrder);
	}

	public void DeleteEntity()
	{
		_prioritizedRecoveredGoodStackRegistry.RemoveStack(PrioritizablePriority, InstantiationOrder);
		_builderPrioritizable.Disable();
	}

	private void OnPriorityChanged(object sender, PriorityChangedEventArgs priorityChangedEventArgs)
	{
		Priority previousPriority = priorityChangedEventArgs.PreviousPriority;
		_prioritizedRecoveredGoodStackRegistry.RemoveStack(previousPriority, InstantiationOrder);
		_prioritizedRecoveredGoodStackRegistry.AddStack(_recoveredGoodStack, PrioritizablePriority, InstantiationOrder);
	}
}
