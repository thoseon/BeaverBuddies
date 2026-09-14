using System.Collections.Generic;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Effects;
using Timberborn.EntityPanelSystem;
using Timberborn.RangedEffectSystem;

namespace Timberborn.Wonders;

internal class WonderEffectBuildingDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private readonly EffectDescriber _effectDescriber;

	private WonderEffectController _wonderEffectController;

	private RangedEffectBuilding _rangedEffectBuilding;

	public WonderEffectBuildingDescriber(EffectDescriber effectDescriber)
	{
		_effectDescriber = effectDescriber;
	}

	public void Awake()
	{
		_wonderEffectController = GetComponent<WonderEffectController>();
		_rangedEffectBuilding = GetComponent<RangedEffectBuilding>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (_wonderEffectController.Effects.Length > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			_effectDescriber.DescribeRangeEffects(_wonderEffectController.Effects, stringBuilder, _rangedEffectBuilding.EffectRadius);
			yield return EntityDescription.CreateTextSection(stringBuilder.ToStringWithoutNewLineEnd(), 1030);
		}
	}
}
