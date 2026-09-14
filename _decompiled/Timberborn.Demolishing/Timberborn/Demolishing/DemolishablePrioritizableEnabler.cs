using Timberborn.BaseComponentSystem;
using Timberborn.BuilderPrioritySystem;

namespace Timberborn.Demolishing;

public class DemolishablePrioritizableEnabler : BaseComponent, IAwakableComponent
{
	private BuilderPrioritizable _builderPrioritizable;

	public void Awake()
	{
		_builderPrioritizable = GetComponent<BuilderPrioritizable>();
		Demolishable component = GetComponent<Demolishable>();
		component.Marked += delegate
		{
			OnMarked();
		};
		component.Unmarked += delegate
		{
			OnUnmarked();
		};
	}

	private void OnMarked()
	{
		_builderPrioritizable.Enable();
	}

	private void OnUnmarked()
	{
		_builderPrioritizable.Disable();
	}
}
