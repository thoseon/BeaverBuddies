using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.NaturalResourcesMoisture;

namespace Timberborn.NaturalResourcesMoistureUI;

internal class WateredNaturalResourceDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string DroughtResistanceLocKey = "NaturalResources.DroughtResistance";

	private readonly ILoc _loc;

	private WateredNaturalResourceSpec _wateredNaturalResource;

	public WateredNaturalResourceDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_wateredNaturalResource = GetComponent<WateredNaturalResourceSpec>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		float daysToDieDry = _wateredNaturalResource.DaysToDieDry;
		if (daysToDieDry > 0f)
		{
			string content = SpecialStrings.RowStarter + _loc.T(DroughtResistanceLocKey, daysToDieDry.ToString("0.#"));
			yield return EntityDescription.CreateTextSection(content, 2050);
		}
	}
}
