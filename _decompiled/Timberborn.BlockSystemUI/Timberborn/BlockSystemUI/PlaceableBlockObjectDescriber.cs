using System.Collections.Generic;
using System.Text;
using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;

namespace Timberborn.BlockSystemUI;

public class PlaceableBlockObjectDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private static readonly string AboveGroundLocKey = "Buildings.AboveGround";

	private static readonly string GroundOnlyLocKey = "Buildings.GroundOnly";

	private static readonly string SolidLocKey = "Buildings.Solid";

	private readonly ILoc _loc;

	private LabeledEntitySpec _labeledEntitySpec;

	private BlockObject _blockObject;

	public PlaceableBlockObjectDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_labeledEntitySpec = GetComponent<LabeledEntitySpec>();
		_blockObject = GetComponent<BlockObject>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		string descriptionLocKey = _labeledEntitySpec.DescriptionLocKey;
		if (!string.IsNullOrEmpty(descriptionLocKey))
		{
			yield return EntityDescription.CreateTextSection(_loc.T(descriptionLocKey), -1);
		}
		EntityDescription entityDescription = DescribeBlockObject();
		if (entityDescription != null)
		{
			yield return entityDescription;
		}
		EntityDescription entityDescription2 = DescribeFlavor();
		if (entityDescription2 != null)
		{
			yield return entityDescription2;
		}
	}

	private EntityDescription DescribeBlockObject()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (_blockObject.Solid)
		{
			stringBuilder.AppendLine(SpecialStrings.RowStarter + _loc.T(SolidLocKey));
		}
		if (_blockObject.GroundOnly)
		{
			stringBuilder.AppendLine(SpecialStrings.RowStarter + _loc.T(GroundOnlyLocKey));
		}
		if (_blockObject.AboveGround)
		{
			stringBuilder.AppendLine(SpecialStrings.RowStarter + _loc.T(AboveGroundLocKey));
		}
		if (stringBuilder.Length <= 0)
		{
			return null;
		}
		return EntityDescription.CreateTextSection(stringBuilder.ToStringWithoutNewLineEnd(), 2000);
	}

	private EntityDescription DescribeFlavor()
	{
		if (_blockObject.IsFinished)
		{
			string flavorDescriptionLocKey = _labeledEntitySpec.FlavorDescriptionLocKey;
			if (!string.IsNullOrEmpty(flavorDescriptionLocKey))
			{
				return EntityDescription.CreateFlavorSection(_loc.T(flavorDescriptionLocKey), 1);
			}
		}
		return null;
	}
}
