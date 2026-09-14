using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using Timberborn.WaterBuildings;

namespace Timberborn.WaterBuildingsUI;

internal class WaterInputPipeSpecDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private readonly ILoc _loc;

	private WaterInputPipeSpec _waterInputPipeSpec;

	private readonly Phrase _maxDepthPhrase = Phrase.New("Work.MaxDepth").FormatDistance<int>();

	public WaterInputPipeSpecDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_waterInputPipeSpec = GetComponent<WaterInputPipeSpec>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		string content = SpecialStrings.RowStarter + _loc.T(_maxDepthPhrase, _waterInputPipeSpec.MaxDepth);
		yield return EntityDescription.CreateTextSection(content, 80);
	}
}
