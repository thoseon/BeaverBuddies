using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.InventorySystem;
using Timberborn.StockpileVisualization;
using UnityEngine;
using UnityEngine.UIElements;

namespace Timberborn.StockpileVisualizationUI;

internal class StockpileGoodColumnVisualizerDebugFragment : IEntityPanelFragment
{
	private readonly DebugFragmentFactory _debugFragmentFactory;

	private readonly VisualElementLoader _visualElementLoader;

	private StockpileGoodColumnVisualizer _stockpileGoodColumnVisualizer;

	private VisualElement _root;

	private TextField _color;

	public StockpileGoodColumnVisualizerDebugFragment(DebugFragmentFactory debugFragmentFactory, VisualElementLoader visualElementLoader)
	{
		_debugFragmentFactory = debugFragmentFactory;
		_visualElementLoader = visualElementLoader;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/StockpileGoodColumnVisualizerDebugFragment";
		VisualElement visualElement = _visualElementLoader.LoadVisualElement(elementName);
		_color = visualElement.Q<TextField>("Color");
		visualElement.Q<Button>("SetColor").RegisterCallback<ClickEvent>(SetColor);
		_root = _debugFragmentFactory.Create("Warehouse box color");
		_root.Q<VisualElement>("Content").Add(visualElement);
		_root.Q<Label>("Text").text = "Put hex color below";
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_stockpileGoodColumnVisualizer = entity.GetComponent<StockpileGoodColumnVisualizer>();
	}

	public void ClearFragment()
	{
		_stockpileGoodColumnVisualizer = null;
	}

	public void UpdateFragment()
	{
		_root.ToggleDisplayStyle(_stockpileGoodColumnVisualizer);
	}

	private void SetColor(ClickEvent evt)
	{
		string text = "#" + _color.value.TrimStart('#');
		if (ColorUtility.TryParseHtmlString(text, out var color))
		{
			_stockpileGoodColumnVisualizer.OverrideColor(color);
			LogColorChange(text);
		}
		else
		{
			Debug.LogWarning("Invalid color: " + text);
		}
	}

	private void LogColorChange(string color)
	{
		Debug.Log(_stockpileGoodColumnVisualizer.GetComponent<SingleGoodAllower>().AllowedGood + " color set to: " + color);
	}
}
