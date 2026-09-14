using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.NaturalResourcesMoisture;

namespace Timberborn.NaturalResourcesMoistureUI;

internal class FloodableNaturalResourceDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string AquaticLocKey = "NaturalResources.Aquatic";

	private readonly ILoc _loc;

	private FloodableNaturalResourceSpec _floodableNaturalResourceSpec;

	public FloodableNaturalResourceDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_floodableNaturalResourceSpec = GetComponent<FloodableNaturalResourceSpec>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (_floodableNaturalResourceSpec.MinWaterHeight > 0)
		{
			string content = SpecialStrings.RowStarter + _loc.T(AquaticLocKey);
			yield return EntityDescription.CreateTextSection(content, 2100);
		}
	}
}
