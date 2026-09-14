using System;
using System.Collections.Generic;
using Timberborn.BaseComponentSystem;
using Timberborn.EntitySystem;
using Timberborn.Goods;
using Timberborn.TemplateAttachmentSystem;
using UnityEngine;

namespace Timberborn.Carrying;

internal class GoodCarrierModel : BaseComponent, IAwakableComponent, IInitializableEntity
{
	private readonly GoodIconVisualizer _goodIconVisualizer;

	private readonly IGoodService _goodService;

	private GoodCarrier _goodCarrier;

	private BackpackCarrier _backpackCarrier;

	private TemplateAttachments _templateAttachments;

	private TemplateAttachmentVisibilityToggle _carriedInHandsToggle;

	private TemplateAttachmentVisibilityToggle _backpackToggle;

	private readonly List<CarriedItem> _itemsInHands = new List<CarriedItem>();

	private readonly List<CarriedItem> _itemsInBackpack = new List<CarriedItem>();

	public GoodCarrierModel(GoodIconVisualizer goodIconVisualizer, IGoodService goodService)
	{
		_goodIconVisualizer = goodIconVisualizer;
		_goodService = goodService;
	}

	public void Awake()
	{
		_goodCarrier = GetComponent<GoodCarrier>();
		_backpackCarrier = GetComponent<BackpackCarrier>();
		_templateAttachments = GetComponent<TemplateAttachments>();
	}

	public void InitializeEntity()
	{
		InitializeItems();
		UpdateItemsVisibility();
		_backpackCarrier.BackpackChanged += OnBackpackChanged;
		_goodCarrier.CarriedGoodsChanged += OnCarriedGoodsChanged;
	}

	private void InitializeItems()
	{
		GoodCarrierModelSpec component = GetComponent<GoodCarrierModelSpec>();
		TemplateAttachment orCreateAttachment = _templateAttachments.GetOrCreateAttachment(component.CarriedInHandsAttachmentName);
		TemplateAttachment orCreateAttachment2 = _templateAttachments.GetOrCreateAttachment(component.BackpackAttachmentName);
		_carriedInHandsToggle = orCreateAttachment.GetVisibilityToggle();
		_backpackToggle = orCreateAttachment2.GetVisibilityToggle();
		_itemsInHands.AddRange(GetItems(orCreateAttachment));
		_itemsInBackpack.AddRange(GetItems(orCreateAttachment2));
	}

	private IEnumerable<CarriedItem> GetItems(TemplateAttachment templateAttachment)
	{
		Transform root = templateAttachment.Transform.GetChild(0);
		int i = 0;
		while (i < root.childCount)
		{
			CarriedItem carriedItem = CarriedItem.CreateLinkedToObject(root.GetChild(i).gameObject, _goodIconVisualizer);
			carriedItem.Hide();
			yield return carriedItem;
			int num = i + 1;
			i = num;
		}
	}

	private void OnBackpackChanged(object sender, EventArgs e)
	{
		UpdateItemsVisibility();
	}

	private void OnCarriedGoodsChanged(object sender, CarriedGoodsChangedEventArgs e)
	{
		UpdateItemsVisibility();
	}

	private void UpdateItemsVisibility()
	{
		if (_backpackCarrier.IsBackpackEnabled)
		{
			_carriedInHandsToggle.Hide();
			_backpackToggle.Show();
			UpdateItems(_itemsInBackpack);
		}
		else
		{
			_backpackToggle.Hide();
			_carriedInHandsToggle.Show();
			UpdateItems(_itemsInHands);
		}
	}

	private void UpdateItems(IEnumerable<CarriedItem> items)
	{
		GoodSpec goodSpec = (_goodCarrier.IsCarrying ? _goodService.GetGood(_goodCarrier.CarriedGoodId) : null);
		VisibleContainer visibleContainer = goodSpec?.VisibleContainer ?? VisibleContainer.None;
		foreach (CarriedItem item in items)
		{
			if (item.VisibleContainer == visibleContainer)
			{
				ShowItem(item, goodSpec);
			}
			else
			{
				item.Hide();
			}
		}
	}

	private static void ShowItem(CarriedItem item, GoodSpec goodSpec)
	{
		if (goodSpec != null)
		{
			item.Show(goodSpec);
		}
		else
		{
			item.Show();
		}
	}
}
