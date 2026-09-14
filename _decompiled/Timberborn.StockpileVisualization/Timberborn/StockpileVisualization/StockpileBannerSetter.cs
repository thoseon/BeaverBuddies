using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using UnityEngine;

namespace Timberborn.StockpileVisualization;

internal class StockpileBannerSetter : BaseComponent, IAwakableComponent, IFinishedStateListener
{
	private static readonly Color BannerIconColor = new Color(0.33f, 0.33f, 0.33f);

	private readonly GoodIconVisualizer _goodIconVisualizer;

	private readonly IGoodService _goodService;

	private BlockObject _blockObject;

	private SingleGoodAllower _singleGoodAllower;

	private MeshRenderer _meshRenderer;

	public StockpileBannerSetter(GoodIconVisualizer goodIconVisualizer, IGoodService goodService)
	{
		_goodIconVisualizer = goodIconVisualizer;
		_goodService = goodService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_singleGoodAllower = GetComponent<SingleGoodAllower>();
		BuildingModel component = GetComponent<BuildingModel>();
		_meshRenderer = component.FinishedModel.GetComponentInChildren<MeshRenderer>();
	}

	public void OnEnterFinishedState()
	{
		_singleGoodAllower.DisallowedGoodsChanged += OnDisallowedGoodsChanged;
		UpdateProperties();
	}

	public void OnExitFinishedState()
	{
		_singleGoodAllower.DisallowedGoodsChanged -= OnDisallowedGoodsChanged;
	}

	public void UpdateProperties()
	{
		if (_singleGoodAllower.HasAllowedGood)
		{
			string allowedGood = _singleGoodAllower.AllowedGood;
			GoodSpec good = _goodService.GetGood(allowedGood);
			_goodIconVisualizer.ShowColoredIcon(_meshRenderer.material, good, _blockObject.FlipMode.IsFlipped, BannerIconColor);
		}
		else
		{
			_goodIconVisualizer.HideColoredIcon(_meshRenderer.material);
		}
	}

	private void OnDisallowedGoodsChanged(object sender, DisallowedGoodsChangedEventArgs e)
	{
		UpdateProperties();
	}
}
