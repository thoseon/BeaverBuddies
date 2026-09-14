using Timberborn.BaseComponentSystem;
using Timberborn.BlockingSystem;

namespace Timberborn.Wonders;

public class BuildingBlockedWonderBlocker : BaseComponent, IAwakableComponent, IWonderBlocker
{
	private BlockableObject _blockableObject;

	public void Awake()
	{
		_blockableObject = GetComponent<BlockableObject>();
	}

	public bool IsWonderBlocked()
	{
		return !_blockableObject.IsUnblocked;
	}
}
