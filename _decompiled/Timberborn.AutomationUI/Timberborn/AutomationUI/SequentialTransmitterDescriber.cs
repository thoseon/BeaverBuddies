using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;

namespace Timberborn.AutomationUI;

internal class SequentialTransmitterDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string SequentialLocKey = "Buildings.Sequential";

	private readonly ILoc _loc;

	private string _text;

	public SequentialTransmitterDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_text = SpecialStrings.RowStarter + _loc.T(SequentialLocKey);
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		yield return EntityDescription.CreateTextSection(_text, 1000);
	}
}
