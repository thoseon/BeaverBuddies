using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Localization;

namespace Timberborn.BlockSystemUI;

public class BlockObjectDeletionDescriber : BaseComponent, IAwakableComponent
{
	private static readonly string DemolishTooltipLocKey = "Demolish.Mark";

	private readonly ILoc _loc;

	private readonly List<IBlockObjectDeletionBlocker> _deletionBlockers = new List<IBlockObjectDeletionBlocker>();

	public BlockObjectDeletionDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		GetComponents(_deletionBlockers);
	}

	public string GetDescription()
	{
		foreach (IBlockObjectDeletionBlocker deletionBlocker in _deletionBlockers)
		{
			if (deletionBlocker.IsDeletionBlocked)
			{
				return _loc.T(deletionBlocker.ReasonLocKey);
			}
		}
		return _loc.T(DemolishTooltipLocKey);
	}
}
