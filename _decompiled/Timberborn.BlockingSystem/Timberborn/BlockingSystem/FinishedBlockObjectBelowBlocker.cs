using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;

namespace Timberborn.BlockingSystem;

internal class FinishedBlockObjectBelowBlocker : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private BlockObjectBelowBlocker _blockObjectBelowBlocker;

	public void Awake()
	{
		_blockObjectBelowBlocker = GetComponent<BlockObjectBelowBlocker>();
	}

	public void OnEnterFinishedState()
	{
		_blockObjectBelowBlocker.Block();
	}

	public void OnExitFinishedState()
	{
		_blockObjectBelowBlocker.Unblock();
	}
}
