using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Explosions;
using Timberborn.Localization;
using Timberborn.UIFormatters;

namespace Timberborn.ExplosionsUI;

public class DynamiteDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private readonly ILoc _loc;

	private Dynamite _dynamite;

	private readonly Phrase _explosionDepthPhrase = Phrase.New("Building.Dynamite.ExplosionDepth").FormatDistance<int>();

	public DynamiteDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_dynamite = GetComponent<Dynamite>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		string content = SpecialStrings.RowStarter + _loc.T(_explosionDepthPhrase, _dynamite.Depth);
		yield return EntityDescription.CreateTextSection(content, 40);
	}
}
