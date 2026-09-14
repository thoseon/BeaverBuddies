using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;

namespace Timberborn.Rendering;

public class StartableMarkerPositionUpdater : BaseComponent, IPostLoadableEntity
{
	public void PostLoadEntity()
	{
		GetComponent<MarkerPosition>().UpdatePosition();
	}
}
