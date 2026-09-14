using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.GameCycleSystem;
using Timberborn.Goods;
using Timberborn.ResourceCountingSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;

namespace Timberborn.GoodsSampling;

internal class GoodsSampler : ILoadableSingleton
{
	private readonly GlobalGoodSamplingRegistry _globalGoodSamplingRegistry;

	private readonly ResourceCountingService _resourceCountingService;

	private readonly IGoodService _goodService;

	private readonly EventBus _eventBus;

	private readonly GameCycleService _gameCycleService;

	private readonly List<DistrictGoodSamplingRegistry> _districtGoodSamplingRegistries = new List<DistrictGoodSamplingRegistry>();

	private readonly Dictionary<string, GoodSample> _globalSamples = new Dictionary<string, GoodSample>();

	public GoodsSampler(GlobalGoodSamplingRegistry globalGoodSamplingRegistry, ResourceCountingService resourceCountingService, IGoodService goodService, EventBus eventBus, GameCycleService gameCycleService)
	{
		_globalGoodSamplingRegistry = globalGoodSamplingRegistry;
		_resourceCountingService = resourceCountingService;
		_goodService = goodService;
		_eventBus = eventBus;
		_gameCycleService = gameCycleService;
	}

	public void AddDistrictRegistry(DistrictGoodSamplingRegistry districtGoodSamplingRegistry)
	{
		_districtGoodSamplingRegistries.Add(districtGoodSamplingRegistry);
	}

	public void RemoveDistrictRegistry(DistrictGoodSamplingRegistry districtGoodSamplingRegistry)
	{
		_districtGoodSamplingRegistries.Remove(districtGoodSamplingRegistry);
	}

	public void Load()
	{
		_eventBus.Register(this);
		ResetGlobalSamples();
	}

	[OnEvent]
	public void OnDaytimeStart(DaytimeStartEvent daytimeStartEvent)
	{
		CollectGoodsSamples();
	}

	[OnEvent]
	public void OnNewGameInitializedEvent(NewGameInitializedEvent newGameInitializedEvent)
	{
		CollectGoodsSamples();
	}

	private void CollectGoodsSamples()
	{
		CollectDistrictsSamples();
		CollectGlobalSamples();
		_eventBus.Post(new GoodsSampledEvent());
	}

	private void ResetGlobalSamples()
	{
		foreach (string good in _goodService.Goods)
		{
			_globalSamples[good] = default(GoodSample);
		}
	}

	private void CollectDistrictsSamples()
	{
		foreach (DistrictGoodSamplingRegistry districtGoodSamplingRegistry in _districtGoodSamplingRegistries)
		{
			DistrictResourceCounter districtResourceCounter = _resourceCountingService.GetDistrictResourceCounter(districtGoodSamplingRegistry.DistrictCenter);
			districtResourceCounter.UpdateCounters();
			DistrictGoodsBalance component = districtGoodSamplingRegistry.DistrictCenter.GetComponent<DistrictGoodsBalance>();
			foreach (string good in _goodService.Goods)
			{
				ResourceCount resourceCount = districtResourceCounter.GetResourceCount(good);
				int production = component.GetProduction(good);
				int consumption = component.GetConsumption(good);
				GoodSample goodSample = new GoodSample(_gameCycleService.Cycle, _gameCycleService.CycleDay, resourceCount.AvailableStock, resourceCount.TotalCapacity, production, consumption);
				districtGoodSamplingRegistry.GoodSamplingRegistry.AddSample(good, goodSample);
				_globalSamples[good] += goodSample;
			}
			component.Flush();
		}
	}

	private void CollectGlobalSamples()
	{
		foreach (var (goodId, goodSample2) in _globalSamples)
		{
			_globalGoodSamplingRegistry.GoodSamplingRegistry.AddSample(goodId, goodSample2);
		}
		ResetGlobalSamples();
	}
}
