using Timberborn.AreaSelectionSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.Buildings;

namespace Timberborn.BuildingsUI;

public class BuildingAreaBoundsDrawingBlocker : BaseComponent, IAwakableComponent
{
	public void Awake()
	{
		BuildingSpec component = GetComponent<BuildingSpec>();
		if ((object)component != null && component.DrawRangeBoundsOnIt)
		{
			GetComponent<AreaBoundsDrawingBlocker>().DisableBlocking();
		}
	}
}
