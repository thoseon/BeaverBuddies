using System;
using Timberborn.Goods;
using UnityEngine;

namespace Timberborn.Carrying;

public class CarriedItem
{
	private static readonly string NameSeparator = ".";

	private readonly GameObject _itemObject;

	private readonly MeshRenderer _meshRenderer;

	private readonly GoodIconVisualizer _goodIconVisualizer;

	public VisibleContainer VisibleContainer { get; }

	private CarriedItem(GameObject itemObject, GoodIconVisualizer goodIconVisualizer, VisibleContainer visibleContainer)
	{
		_itemObject = itemObject;
		_goodIconVisualizer = goodIconVisualizer;
		_meshRenderer = _itemObject.GetComponent<MeshRenderer>();
		VisibleContainer = visibleContainer;
	}

	public static CarriedItem CreateLinkedToObject(GameObject itemObject, GoodIconVisualizer goodIconVisualizer)
	{
		VisibleContainer visibleContainer = ParseGoodContainerFromName(itemObject);
		return new CarriedItem(itemObject, goodIconVisualizer, visibleContainer);
	}

	public void Show(GoodSpec goodSpec)
	{
		Show();
		UpdateProperties(goodSpec);
	}

	public void Show()
	{
		SetVisibility(show: true);
	}

	public void Hide()
	{
		SetVisibility(show: false);
	}

	private void SetVisibility(bool show)
	{
		_itemObject.SetActive(show);
	}

	private void UpdateProperties(GoodSpec goodSpec)
	{
		_goodIconVisualizer.ShowIcon(_meshRenderer.material, goodSpec);
	}

	private static VisibleContainer ParseGoodContainerFromName(UnityEngine.Object itemObject)
	{
		string name = itemObject.name;
		int num = name.LastIndexOf(NameSeparator, StringComparison.Ordinal);
		if (num < 0)
		{
			throw new ArgumentException("Invalid carried object name: " + name);
		}
		string value = name.Substring(num + NameSeparator.Length);
		return (VisibleContainer)Enum.Parse(typeof(VisibleContainer), value);
	}
}
