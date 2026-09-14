using Timberborn.BaseComponentSystem;
using Timberborn.Characters;
using Timberborn.EntityPanelSystem;
using Timberborn.GameDistricts;
using Timberborn.GameFactionSystem;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using UnityEngine;

namespace Timberborn.BotsUI;

internal class BotEntityBadge : BaseComponent, IAwakableComponent, IEntityBadge
{
	private static readonly string AgeLocKey = "Beaver.Age";

	private static readonly string DeadNameSuffixLocKey = "Bot.DeadNameSuffix";

	private readonly ILoc _loc;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly FactionService _factionService;

	private Character _character;

	private Citizen _citizen;

	public int EntityBadgePriority => 20;

	public BotEntityBadge(ILoc loc, EntitySelectionService entitySelectionService, FactionService factionService)
	{
		_loc = loc;
		_entitySelectionService = entitySelectionService;
		_factionService = factionService;
	}

	public void Awake()
	{
		_character = GetComponent<Character>();
		_citizen = GetComponent<Citizen>();
	}

	public string GetEntitySubtitle()
	{
		string text = Age();
		if (!_character.Alive)
		{
			return text + " " + _loc.T(DeadNameSuffixLocKey);
		}
		return text;
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
		return _factionService.Current.BotAvatar.Asset;
	}

	private string Age()
	{
		return _loc.T(AgeLocKey, _character.Age);
	}
}
