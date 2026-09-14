using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuilderPrioritySystem;
using Timberborn.PrioritySystem;
using Timberborn.TemplateSystem;

namespace Timberborn.ConstructionSites;

internal class ConstructionRegistrar : BaseComponent, IAwakableComponent, IUnfinishedStateListener
{
	private readonly ConstructionRegistry _constructionRegistry;

	private ConstructionJob _constructionJob;

	private InstantiatedTemplate _instantiatedTemplate;

	private BuilderPrioritizable _builderPrioritizable;

	private int InstantiationOrder => _instantiatedTemplate.InstantiationOrder;

	private Priority Priority => _builderPrioritizable.Priority;

	public ConstructionRegistrar(ConstructionRegistry constructionRegistry)
	{
		_constructionRegistry = constructionRegistry;
	}

	public void Awake()
	{
		_constructionJob = GetComponent<ConstructionJob>();
		_instantiatedTemplate = GetComponent<InstantiatedTemplate>();
		_builderPrioritizable = GetComponent<BuilderPrioritizable>();
	}

	public void OnEnterUnfinishedState()
	{
		_constructionRegistry.AddJob(_constructionJob, Priority, InstantiationOrder);
		_builderPrioritizable.PriorityChanged += OnPriorityChanged;
	}

	public void OnExitUnfinishedState()
	{
		_constructionRegistry.RemoveJob(Priority, InstantiationOrder);
		_builderPrioritizable.PriorityChanged -= OnPriorityChanged;
	}

	private void OnPriorityChanged(object sender, PriorityChangedEventArgs priorityChangedEventArgs)
	{
		Priority previousPriority = priorityChangedEventArgs.PreviousPriority;
		_constructionRegistry.RemoveJob(previousPriority, InstantiationOrder);
		_constructionRegistry.AddJob(_constructionJob, Priority, InstantiationOrder);
	}
}
