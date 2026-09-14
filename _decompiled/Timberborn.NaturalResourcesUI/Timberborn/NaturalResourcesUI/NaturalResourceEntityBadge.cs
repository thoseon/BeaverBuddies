using Timberborn.BaseComponentSystem;
using Timberborn.Cutting;
using Timberborn.EntityPanelSystem;
using Timberborn.EntitySystem;
using Timberborn.Growing;
using Timberborn.Localization;
using UnityEngine;

namespace Timberborn.NaturalResourcesUI;

internal class NaturalResourceEntityBadge : BaseComponent, IAwakableComponent, IEntityBadge
{
	private static readonly string SeedlingLocKey = "NaturalResources.Seedling";

	private static readonly string LeftoverLocKey = "NaturalResources.Leftover";

	private readonly ILoc _loc;

	private LabeledEntity _labeledEntity;

	private Growable _growable;

	private Cuttable _cuttable;

	public int EntityBadgePriority => 200;

	public NaturalResourceEntityBadge(ILoc loc)
	{
		_loc = loc;
	}

	public void Awake()
	{
		_labeledEntity = GetComponent<LabeledEntity>();
		_growable = GetComponent<Growable>();
		_cuttable = GetComponent<Cuttable>();
	}

	public string GetEntitySubtitle()
	{
		if ((bool)_cuttable && _cuttable.Yielder.IsYieldRemoved)
		{
			return _loc.T(LeftoverLocKey);
		}
		if ((bool)_growable && !_growable.IsGrown)
		{
			return _loc.T(SeedlingLocKey);
		}
		return "";
	}

	public ClickableSubtitle GetEntityClickableSubtitle()
	{
		return ClickableSubtitle.CreateEmpty();
	}

	public Sprite GetEntityAvatar()
	{
		return _labeledEntity.Image;
	}
}
