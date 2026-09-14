using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.EntitySystem;

namespace Timberborn.BlockObjectAccesses;

public class HighBlockObjectAccessesAdder : BaseComponent, IInitializableEntity
{
	public void InitializeEntity()
	{
		int z = GetComponent<BlockObject>().Blocks.Size.z;
		GetComponent<BlockObjectAccessible>().SetNumberOfAccessLevelsAboveGround(z);
	}
}
