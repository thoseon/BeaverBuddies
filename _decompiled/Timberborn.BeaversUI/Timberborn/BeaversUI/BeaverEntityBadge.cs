using Timberborn.BaseComponentSystem;
using Timberborn.BeaverContaminationSystem;
using Timberborn.Beavers;
using Timberborn.Characters;
using Timberborn.EntityPanelSystem;
using Timberborn.GameDistricts;
using Timberborn.GameFactionSystem;
using Timberborn.Localization;
using Timberborn.MortalSystem;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.BeaversUI;

internal class BeaverEntityBadge : BaseComponent, IAwakableComponent, IEntityBadge
{
	private static readonly string AgeLocKey = "Beaver.Age";

	private static readonly string DeadNameSuffixLocKey = "Beaver.DeadNameSuffix";

	private readonly FactionService _factionService;

	private readonly ILoc _loc;

	private readonly EntitySelectionService _entitySelectionService;

	private Mortal _mortal;

	private Child _child;

	private Character _character;

	private Citizen _citizen;

	private Contaminable _contaminable;

	public int EntityBadgePriority => 10;

	public BeaverEntityBadge(FactionService factionService, ILoc loc, EntitySelectionService entitySelectionService)
	{
		_factionService = factionService;
		_loc = loc;
		_entitySelectionService = entitySelectionService;
	}

	public void Awake()
	{
		_mortal = GetComponent<Mortal>();
		_child = GetComponent<Child>();
		_character = GetComponent<Character>();
		_citizen = GetComponent<Citizen>();
		_contaminable = GetComponent<Contaminable>();
	}

	public string GetEntitySubtitle()
	{
		string text = Age();
		if (!_mortal.Dead)
		{
			return text;
		}
		return text + " " + _loc.T(DeadNameSuffixLocKey);
	}

	public ClickableSubtitle GetEntityClickableSubtitle()
	{
		if (_citizen.HasAssignedDistrict)
		{
			DistrictCenter district = _citizen.AssignedDistrict;
			return ClickableSubtitle.Create(delegate
			{
				_entitySelectionService.SelectAndFocusOn(district);
			}, district.DistrictName);
		}
		return ClickableSubtitle.CreateEmpty();
	}

	public Sprite GetEntityAvatar()
	{
		if ((bool)_contaminable && _contaminable.IsContaminated)
		{
			if (!(BaseComponent)(object)_child)
			{
				return _factionService.Current.ContaminatedAdultAvatar.Asset;
			}
			return _factionService.Current.ContaminatedChildAvatar.Asset;
		}
		if (!(BaseComponent)(object)_child)
		{
			return _factionService.Current.Avatar.Asset;
		}
		return _factionService.Current.ChildAvatar.Asset;
	}

	private string Age()
	{
		return _loc.T(AgeLocKey, _character.Age);
	}
}
