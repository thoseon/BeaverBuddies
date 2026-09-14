using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.MechanicalSystem;
using Timberborn.UIFormatters;
using UnityEngine.UIElements;

namespace Timberborn.MechanicalSystemUI;

internal class MechanicalNodeDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string PowerClass = "described-amount--power";

	private readonly ILoc _loc;

	private readonly DescribedAmountFactory _describedAmountFactory;

	private readonly ProductionItemFactory _productionItemFactory;

	private MechanicalNode _mechanicalNode;

	private MechanicalNodeSpec _mechanicalNodeSpec;

	private MechanicalNodeDescriptionSpec _mechanicalNodeDescriptionSpec;

	private Phrase _powerInputPhrase;

	private Phrase _powerOutputPhrase;

	private readonly Phrase _powerInputTooltipPhrase = Phrase.New("Mechanical.PowerInput");

	private readonly Phrase _powerOutputTooltipPhrase = Phrase.New("Mechanical.PowerOutput");

	public MechanicalNodeDescriber(ILoc loc, DescribedAmountFactory describedAmountFactory, ProductionItemFactory productionItemFactory)
	{
		_loc = loc;
		_describedAmountFactory = describedAmountFactory;
		_productionItemFactory = productionItemFactory;
	}

	public void Awake()
	{
		_mechanicalNode = GetComponent<MechanicalNode>();
		_mechanicalNodeSpec = GetComponent<MechanicalNodeSpec>();
		_mechanicalNodeDescriptionSpec = GetComponent<MechanicalNodeDescriptionSpec>();
		_powerInputPhrase = Phrase.New().FormatPower<int>(GetPowerUnitLocKey);
		_powerOutputPhrase = Phrase.New().FormatPower<int>(GetPowerUnitLocKey);
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (_mechanicalNodeSpec != null && !_mechanicalNode.Enabled)
		{
			int powerOutput = _mechanicalNodeSpec.PowerOutput;
			int powerInput = _mechanicalNodeSpec.PowerInput;
			if (powerOutput > 0)
			{
				string text = _loc.T(_powerOutputPhrase, powerOutput);
				string tooltip = _loc.T(_powerOutputTooltipPhrase, text);
				VisualElement output = _describedAmountFactory.CreatePlain(PowerClass, text, tooltip);
				VisualElement content = _productionItemFactory.CreateOutput(output);
				yield return EntityDescription.CreateOutputSection(content, 2147483646);
			}
			if (powerInput > 0)
			{
				string text2 = _loc.T(_powerInputPhrase, powerInput);
				string tooltip2 = _loc.T(_powerInputTooltipPhrase, text2);
				VisualElement content2 = _describedAmountFactory.CreatePlain(PowerClass, text2, tooltip2);
				yield return EntityDescription.CreateMiddleSection(content2, 4);
			}
		}
	}

	private string GetPowerUnitLocKey()
	{
		return _mechanicalNodeDescriptionSpec?.AlternativePowerUnitLocKey;
	}
}
