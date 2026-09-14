using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.ReservableSystem;
using Timberborn.Yielding;

namespace Timberborn.Forestry;

public class TreeRemoveYieldStrategy : BaseComponent, IAwakableComponent, IRemoveYieldStrategy
{
	private readonly TreeCuttingArea _treeCuttingArea;

	private BlockObject _blockObject;

	private readonly List<Yielder> _yielders = new List<Yielder>();

	public ReservableReacher Reacher { get; private set; }

	public string Id => "Cuttable";

	public bool IsStillRemovable
	{
		get
		{
			if (_treeCuttingArea.IsInCuttingArea(_blockObject.Coordinates))
			{
				return AnyYielderIsYielding();
			}
			return false;
		}
	}

	public event EventHandler<TreeCutter> CuttingStarted;

	public event EventHandler CuttingStopped;

	public TreeRemoveYieldStrategy(TreeCuttingArea treeCuttingArea)
	{
		_treeCuttingArea = treeCuttingArea;
	}

	public void Awake()
	{
		Reacher = GetComponent<TreeReacher>();
		_blockObject = GetComponent<BlockObject>();
		GetComponents(_yielders);
	}

	public void StartCutting(TreeCutter treeCutter)
	{
		CuttingStarted?.Invoke(this, treeCutter);
	}

	public void StopCutting()
	{
		CuttingStopped?.Invoke(this, EventArgs.Empty);
	}

	private bool AnyYielderIsYielding()
	{
		foreach (Yielder yielder in _yielders)
		{
			if (yielder.IsYielding)
			{
				return true;
			}
		}
		return false;
	}
}
