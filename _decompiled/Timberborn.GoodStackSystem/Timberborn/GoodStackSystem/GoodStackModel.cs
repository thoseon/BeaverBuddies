using System.Collections.Generic;
using System.Linq;
using Timberborn.BaseComponentSystem;
using Timberborn.Common;
using Timberborn.Goods;
using Timberborn.InventorySystem;
using UnityEngine;

namespace Timberborn.GoodStackSystem;

internal class GoodStackModel : BaseComponent
{
	private readonly GoodIconVisualizer _goodIconVisualizer;

	private readonly IGoodService _goodService;

	private readonly List<GoodSpec> _goods = new List<GoodSpec>();

	private MeshRenderer _boxMeshRenderer;

	private MeshRenderer _barrelMeshRenderer;

	private GameObject _root;

	private GameObject _log;

	private GameObject _barrel;

	private GameObject _box;

	private GameObject _bag;

	public GoodStackModel(GoodIconVisualizer goodIconVisualizer, IGoodService goodService)
	{
		_goodIconVisualizer = goodIconVisualizer;
		_goodService = goodService;
	}

	public void Initialize(GameObject root, GoodStackModelSpec spec)
	{
		_root = root;
		_log = _root.FindChild(spec.LogObjectName);
		_barrel = _root.FindChild(spec.BarrelObjectName);
		_box = _root.FindChild(spec.BoxObjectName);
		_bag = _root.FindChild(spec.BagObjectName);
		_boxMeshRenderer = _box.GetComponentInChildren<MeshRenderer>();
		_barrelMeshRenderer = _barrel.GetComponentInChildren<MeshRenderer>();
	}

	public void UpdateModel(Inventory inventory)
	{
		_root.SetActive(value: true);
		foreach (GoodAmount item in inventory.Stock)
		{
			_goods.Add(_goodService.GetGood(item.GoodId));
		}
		ToggleActive(_log, VisibleContainer.Log);
		ToggleActive(_barrel, VisibleContainer.Barrel);
		ToggleActive(_box, VisibleContainer.Box);
		ToggleActive(_bag, VisibleContainer.Bag);
		UpdateIcon(_boxMeshRenderer, VisibleContainer.Box);
		UpdateIcon(_barrelMeshRenderer, VisibleContainer.Barrel);
		_goods.Clear();
	}

	private void ToggleActive(GameObject item, VisibleContainer visibleContainer)
	{
		bool state = _goods.Any((GoodSpec good) => good.VisibleContainer == visibleContainer);
		ToggleActive(item, state);
	}

	private static void ToggleActive(GameObject item, bool state)
	{
		item.SetActive(state);
	}

	private void UpdateIcon(MeshRenderer meshRenderer, VisibleContainer visibleContainer)
	{
		GoodSpec goodSpec = _goods.FirstOrDefault((GoodSpec good) => good.VisibleContainer == visibleContainer);
		if (goodSpec != null)
		{
			_goodIconVisualizer.ShowIcon(meshRenderer.material, goodSpec);
		}
	}
}
