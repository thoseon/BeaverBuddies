using System.Collections.Generic;
using Timberborn.BlockObjectModelSystem;
using Timberborn.BlockSystem;

namespace Timberborn.BlockObjectPickingSystem;

public class BlockObjectModelBlockadeIgnorer
{
	private readonly List<BlockObjectModelController> _blockObjectModelControllers = new List<BlockObjectModelController>();

	public void IgnoreModelBlockades(IEnumerable<BlockObject> blockObjects)
	{
		foreach (BlockObject blockObject in blockObjects)
		{
			BlockObjectModelController component = blockObject.GetComponent<BlockObjectModelController>();
			if ((bool)component)
			{
				component.IgnoreModelBlockade();
				_blockObjectModelControllers.Add(component);
			}
		}
	}

	public void UnignoreModelBlockades()
	{
		foreach (BlockObjectModelController blockObjectModelController in _blockObjectModelControllers)
		{
			blockObjectModelController.UnignoreModelBlockade();
		}
		Clear();
	}

	public void Clear()
	{
		_blockObjectModelControllers.Clear();
	}
}
