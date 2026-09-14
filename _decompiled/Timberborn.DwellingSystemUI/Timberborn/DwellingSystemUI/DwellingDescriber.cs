using System.Collections.Generic;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.DwellingSystem;
using Timberborn.Effects;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;

namespace Timberborn.DwellingSystemUI;

public class DwellingDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string InhabitantsLocKey = "Dwelling.Inhabitants";

	private readonly EffectDescriber _effectDescriber;

	private readonly ILoc _loc;

	private Dwelling _dwelling;

	private readonly StringBuilder _description = new StringBuilder();

	public DwellingDescriber(EffectDescriber effectDescriber, ILoc loc)
	{
		_effectDescriber = effectDescriber;
		_loc = loc;
	}

	public void Awake()
	{
		_dwelling = GetComponent<Dwelling>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (!_dwelling.Enabled)
		{
			string text = _loc.T(InhabitantsLocKey, _dwelling.MaxBeavers);
			string content = SpecialStrings.RowStarter + text;
			yield return EntityDescription.CreateTextSection(content, 30);
		}
		if (_dwelling.SleepEffects.Count > 0)
		{
			_effectDescriber.DescribeEffects(_dwelling.SleepEffects, _description);
			yield return EntityDescription.CreateTextSection(_description.ToStringWithoutNewLineEndAndClean(), 1000);
		}
	}
}
