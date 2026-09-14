using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockObjectModelSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;

namespace Timberborn.BlockSystemUI;

internal class UndergroundDepthDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string UndergroundDepthLocKey = "Buildings.UndergroundDepth";

	private static readonly string InfiniteDepthLocKey = "Buildings.InfiniteDepth";

	private readonly ILoc _loc;

	private UndergroundDepthDescriberSpec _undergroundDepthDescriberSpec;

	private IInfiniteUndergroundModel _infiniteUndergroundModel;

	private readonly Phrase _undergroundDepthPhrase = Phrase.New().FormatDistance<int>();

	public UndergroundDepthDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_undergroundDepthDescriberSpec = GetComponent<UndergroundDepthDescriberSpec>();
		_infiniteUndergroundModel = GetComponent<IInfiniteUndergroundModel>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		string param = ((_infiniteUndergroundModel != null) ? _loc.T(InfiniteDepthLocKey) : _loc.T(_undergroundDepthPhrase, _undergroundDepthDescriberSpec.Depth));
		yield return EntityDescription.CreateTextSection(SpecialStrings.RowStarter + _loc.T(UndergroundDepthLocKey, param), 2200);
	}
}
