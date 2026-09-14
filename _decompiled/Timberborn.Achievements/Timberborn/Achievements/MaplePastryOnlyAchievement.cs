using System.Collections.Generic;
using System.Linq;
using Timberborn.AchievementSystem;
using Timberborn.GameFactionSystem;
using Timberborn.Goods;
using Timberborn.ResourceCountingSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace Timberborn.Achievements;

internal class MaplePastryOnlyAchievement : Achievement
{
	private static readonly string FoodGroupId = "Food";

	private static readonly string RequiredGoodId = "MaplePastry";

	private static readonly int RequiredGoodCount = 1000;

	private readonly IGoodService _goodService;

	private readonly EventBus _eventBus;

	private readonly ResourceCountingService _resourceCountingService;

	private readonly FactionService _factionService;

	private readonly List<string> _forbiddenGoods = new List<string>();

	public override string Id => "MAPLE_PASTRY_ONLY";

	public MaplePastryOnlyAchievement(IGoodService goodService, EventBus eventBus, ResourceCountingService resourceCountingService, FactionService factionService)
	{
		_goodService = goodService;
		_eventBus = eventBus;
		_resourceCountingService = resourceCountingService;
		_factionService = factionService;
	}

	[OnEvent]
	public void OnNighttimeStart(NighttimeStartEvent nighttimeStartEvent)
	{
		if (HasRequiredGood() && !HasAnyForbiddenGood())
		{
			Unlock();
		}
	}

	protected override void EnableInternal()
	{
		if (_factionService.Current.Id == AchievementHelper.Folktails)
		{
			_eventBus.Register(this);
			_forbiddenGoods.AddRange(from good in _goodService.GetGoodsForGroup(FoodGroupId)
				where good != RequiredGoodId
				select good);
		}
	}

	protected override void DisableInternal()
	{
		_eventBus.Unregister(this);
	}

	private bool HasRequiredGood()
	{
		return _resourceCountingService.GetGlobalResourceCount(RequiredGoodId).AvailableStock >= RequiredGoodCount;
	}

	private bool HasAnyForbiddenGood()
	{
		foreach (string forbiddenGood in _forbiddenGoods)
		{
			if (_resourceCountingService.GetGlobalResourceCount(forbiddenGood).AvailableStock > 0)
			{
				return true;
			}
		}
		return false;
	}
}
