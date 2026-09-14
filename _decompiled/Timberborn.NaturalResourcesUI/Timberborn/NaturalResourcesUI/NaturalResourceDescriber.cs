using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using Timberborn.Localization;

namespace Timberborn.NaturalResourcesUI;

public class NaturalResourceDescriber : BaseComponent, IAwakableComponent, IEntityDescriber
{
	private readonly ILoc _loc;

	private LabeledEntitySpec _labeledEntitySpec;

	public NaturalResourceDescriber(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_labeledEntitySpec = GetComponent<LabeledEntitySpec>();
	}

	public IEnumerable<EntityDescription> DescribeEntity()
	{
		if (base.GameObject.activeSelf)
		{
			string flavorDescriptionLocKey = _labeledEntitySpec.FlavorDescriptionLocKey;
			if (!string.IsNullOrEmpty(flavorDescriptionLocKey))
			{
				yield return EntityDescription.CreateFlavorSection(_loc.T(flavorDescriptionLocKey), 2);
			}
		}
	}
}
