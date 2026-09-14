using System.Collections.Generic;
using System.Text;
using Timberborn.Attractions;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Effects;
using Timberborn.EnterableSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;

namespace Timberborn.AttractionsUI;

public class AttractionDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string VisitorsLimitLocKey = "Attractions.VisitorsLimit";

	private readonly EffectDescriber _effectDescriber;

	private readonly ILoc _loc;

	private Attraction _attraction;

	private Enterable _enterable;

	private readonly StringBuilder _description = new StringBuilder();

	public AttractionDescriber(EffectDescriber effectDescriber, ILoc loc)
	{
		_effectDescriber = effectDescriber;
		_loc = loc;
	}

	public void Awake()
	{
		_attraction = GetComponent<Attraction>();
		_enterable = GetComponent<Enterable>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (!_enterable.Enabled)
		{
			string text = _loc.T(VisitorsLimitLocKey, _enterable.EnterableSpec.CapacityFinished);
			yield return EntityDescription.CreateTextSection(SpecialStrings.RowStarter + text, 10);
		}
		if (_attraction.Effects.Count > 0)
		{
			_effectDescriber.DescribeEffects(_attraction.Effects, _description);
			yield return EntityDescription.CreateTextSection(_description.ToStringWithoutNewLineEndAndClean(), 1010);
		}
	}
}
