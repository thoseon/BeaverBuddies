using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;

namespace Timberborn.BlockSystem;

internal class BlockObjectPostLoadState : BaseComponent, IAwakableComponent, IPostLoadableEntity, IFinishedStateListener
{
	private BlockObject _blockObject;

	private bool _postLoaded;

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public void PostLoadEntity()
	{
		if (_blockObject.IsFinished)
		{
			NotifyEnter();
		}
		_postLoaded = true;
	}

	public void OnEnterFinishedState()
	{
		if (_postLoaded)
		{
			NotifyEnter();
		}
	}

	public void OnExitFinishedState()
	{
		if (_postLoaded)
		{
			NotifyExit();
		}
	}

	private void NotifyEnter()
	{
		foreach (IFinishedPostLoadStateListener item in GetComponentsAllocating<IFinishedPostLoadStateListener>())
		{
			item.OnEnterFinishedPostLoadState();
		}
	}

	private void NotifyExit()
	{
		foreach (IFinishedPostLoadStateListener item in GetComponentsAllocating<IFinishedPostLoadStateListener>())
		{
			item.OnExitFinishedPostLoadState();
		}
	}
}
