using System.Collections.Generic;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Effects;
using Timberborn.EntityPanelSystem;

namespace Timberborn.RangedEffectSystem;

internal class ContinuousEffectBuildingDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private readonly EffectDescriber _effectDescriber;

	private ContinuousEffectBuilding _continuousEffectBuilding;

	private RangedEffectBuilding _rangedEffectBuilding;

	public ContinuousEffectBuildingDescriber(EffectDescriber effectDescriber)
	{
		_effectDescriber = effectDescriber;
	}

	public void Awake()
	{
		_continuousEffectBuilding = GetComponent<ContinuousEffectBuilding>();
		_rangedEffectBuilding = GetComponent<RangedEffectBuilding>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (_continuousEffectBuilding.Effects.Length > 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			_effectDescriber.DescribeRangeEffects(_continuousEffectBuilding.Effects, stringBuilder, _rangedEffectBuilding.EffectRadius);
			yield return EntityDescription.CreateTextSection(stringBuilder.ToStringWithoutNewLineEnd(), 1020);
		}
	}
}
