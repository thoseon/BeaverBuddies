using Timberborn.BaseComponentSystem;

namespace Timberborn.MortalComponents;

public class DeadComponentDisabler
{
	public void DisableComponentsDeadDoNotNeed(BaseComponent entity)
	{
		foreach (object allComponent in entity.AllComponents)
		{
			if (!(allComponent is IDeadNeededComponent) && allComponent is BaseComponent baseComponent)
			{
				baseComponent.DisableComponent();
			}
		}
	}
}
