using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.NaturalResourcesMoisture;

namespace Timberborn.NaturalResourcesMoistureUI;

internal class AridNaturalResourceDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string OverwateringResistanceLocKey = "NaturalResources.OverwateringResistance";

	private static readonly string AridLocKey = "NaturalResources.Arid";

	private readonly ILoc _loc;

	private AridNaturalResourceSpec _aridNaturalResource;

	public AridNaturalResourceDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_aridNaturalResource = GetComponent<AridNaturalResourceSpec>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		float daysToDieWet = _aridNaturalResource.DaysToDieWet;
		string text = _loc.T(OverwateringResistanceLocKey, daysToDieWet.ToString("0.#")) ?? "";
		string content = SpecialStrings.RowStarter + text;
		yield return EntityDescription.CreateTextSection(content, 2050);
		string content2 = SpecialStrings.RowStarter + _loc.T(AridLocKey);
		yield return EntityDescription.CreateTextSection(content2, 2060);
	}
}
