using Timberborn.BaseComponentSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using UnityEngine;

namespace Timberborn.AutomationBuildings;

internal class ResourceCounterBannerSetter : BaseComponent, IAwakableComponent, IInitializablePreview, IInitializableEntity
{
	private static readonly Color BannerIconColor = new Color(0.33f, 0.33f, 0.33f);

	private readonly GoodIconVisualizer _goodIconVisualizer;

	private readonly IGoodService _goodService;

	private BlockObject _blockObject;

	private ResourceCounter _resourceCounter;

	private MeshRenderer _meshRenderer;

	public ResourceCounterBannerSetter(GoodIconVisualizer goodIconVisualizer, IGoodService goodService)
	{
		_goodIconVisualizer = goodIconVisualizer;
		_goodService = goodService;
	}

	public void Awake()
	{
		_blockObject = GetComponent<BlockObject>();
		_resourceCounter = GetComponent<ResourceCounter>();
		BuildingModel component = GetComponent<BuildingModel>();
		_meshRenderer = component.FinishedModel.GetComponentInChildren<MeshRenderer>();
	}

	public void InitializePreview()
	{
		UpdateProperties();
	}

	public void InitializeEntity()
	{
		_resourceCounter.GoodChanged += OnGoodChanged;
		UpdateProperties();
	}

	private void OnGoodChanged(object sender, string e)
	{
		UpdateProperties();
	}

	private void UpdateProperties()
	{
		string goodId = _resourceCounter.GoodId;
		if (string.IsNullOrWhiteSpace(goodId))
		{
			_goodIconVisualizer.HideColoredIcon(_meshRenderer.material);
			return;
		}
		GoodSpec good = _goodService.GetGood(goodId);
		_goodIconVisualizer.ShowColoredIcon(_meshRenderer.material, good, _blockObject.FlipMode.IsFlipped, BannerIconColor);
	}
}
