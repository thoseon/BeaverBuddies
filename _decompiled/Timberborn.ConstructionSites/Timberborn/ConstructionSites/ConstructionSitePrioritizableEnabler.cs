using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.BuilderPrioritySystem;

namespace Timberborn.ConstructionSites;

internal class ConstructionSitePrioritizableEnabler : BaseComponent, IAwakableComponent, IUnfinishedStateListener
{
	private BuilderPrioritizable _builderPrioritizable;

	public void Awake()
	{
		_builderPrioritizable = GetComponent<BuilderPrioritizable>();
	}

	public void OnEnterUnfinishedState()
	{
		_builderPrioritizable.Enable();
	}

	public void OnExitUnfinishedState()
	{
		_builderPrioritizable.Disable();
	}
}
