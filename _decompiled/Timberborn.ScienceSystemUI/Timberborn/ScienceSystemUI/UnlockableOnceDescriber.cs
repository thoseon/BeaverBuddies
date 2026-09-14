using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;

namespace Timberborn.ScienceSystemUI;

public class UnlockableOnceDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string UnlockableOnceLocKey = "Science.UnlockableOnce";

	private readonly ILoc _loc;

	private BlockObject _blockObject;

	public UnlockableOnceDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (_blockObject.IsPreview)
		{
			string text = _loc.T(UnlockableOnceLocKey);
			yield return EntityDescription.CreateTextSection(SpecialStrings.RowStarter + text, 3000);
		}
	}
}
