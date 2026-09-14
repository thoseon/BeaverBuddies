using System.Collections.Generic;
using System.Linq;
using Timberborn.CoreUI;
using Timberborn.GameDistricts;
using Timberborn.Goods;
using Timberborn.SingletonSystem;
using Timberborn.UILayoutSystem;
using UnityEngine.UIElements;

namespace Timberborn.TopBarSystem;

internal class TopBarPanel : IUpdatableSingleton, IPostLoadableSingleton
{
	private static readonly string PanelDistrictClass = "panel--district";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly UILayout _uiLayout;

	private readonly GoodsGroupSpecService _goodsGroupSpecService;

	private readonly IGoodService _goodService;

	private readonly TopBarCounterFactory _topBarCounterFactory;

	private readonly DistrictContextService _districtContextService;

	private readonly EventBus _eventBus;

	private VisualElement _root;

	private readonly List<ITopBarCounter> _counters = new List<ITopBarCounter>();

	public TopBarPanel(VisualElementLoader visualElementLoader, UILayout uiLayout, GoodsGroupSpecService goodsGroupSpecService, IGoodService goodService, TopBarCounterFactory topBarCounterFactory, DistrictContextService districtContextService, EventBus eventBus)
	{
		_visualElementLoader = visualElementLoader;
		_uiLayout = uiLayout;
		_goodsGroupSpecService = goodsGroupSpecService;
		_goodService = goodService;
		_topBarCounterFactory = topBarCounterFactory;
		_districtContextService = districtContextService;
		_eventBus = eventBus;
	}

	public void UpdateSingleton()
	{
		UpdatePanel();
	}

	public void PostLoad()
	{
		_root = _visualElementLoader.LoadVisualElement("Game/TopBar/TopBarPanel");
		foreach (GoodGroupSpec goodGroupSpec in _goodsGroupSpecService.GoodGroupSpecs)
		{
			_counters.Add(CreateCounter(goodGroupSpec));
		}
		UpdatePanel();
		_eventBus.Register(this);
	}

	[OnEvent]
	public void OnShowPrimaryUI(ShowPrimaryUIEvent showPrimaryUIEvent)
	{
		_uiLayout.AddTopBar(_root);
	}

	private ITopBarCounter CreateCounter(GoodGroupSpec goodGroupSpec)
	{
		if (goodGroupSpec.SingleResourceGroup)
		{
			string goodId = _goodService.Goods.Single((string good) => IsGroupGood(goodGroupSpec, good));
			return _topBarCounterFactory.CreateSimpleCounter(goodGroupSpec, goodId, _root);
		}
		IEnumerable<string> goods = _goodService.Goods.Where((string good) => IsGroupGood(goodGroupSpec, good));
		return _topBarCounterFactory.CreateExtendableCounter(goodGroupSpec, goods, _root);
	}

	private void UpdatePanel()
	{
		UpdateCounters();
		_root.EnableInClassList(PanelDistrictClass, _districtContextService.SelectedDistrict);
	}

	private void UpdateCounters()
	{
		foreach (ITopBarCounter counter in _counters)
		{
			counter.UpdateValues();
		}
	}

	private bool IsGroupGood(GoodGroupSpec goodGroupSpec, string good)
	{
		return _goodService.GetGood(good).GoodGroupId == goodGroupSpec.Id;
	}
}
