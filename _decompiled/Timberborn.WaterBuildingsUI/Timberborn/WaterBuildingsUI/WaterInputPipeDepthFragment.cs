using System;
using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using Timberborn.UIFormatters;
using Timberborn.WaterBuildings;
using UnityEngine.UIElements;

namespace Timberborn.WaterBuildingsUI;

internal class WaterInputPipeDepthFragment : IEntityPanelFragment
{
	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private WaterInputPipeCoordinates _waterInputPipeCoordinates;

	private WaterInputPipeSpec _waterInputPipeSpec;

	private VisualElement _root;

	private Label _depth;

	private Label _limit;

	private Button _increaseDepth;

	private Button _decreaseDepth;

	private Toggle _useDepthLimit;

	private readonly Phrase _limitPhrase = Phrase.New("WaterInputCoordinates.Depth").FormatDistance<int>();

	private readonly Phrase _depthPhrase = Phrase.New("WaterInputCoordinates.Limit").FormatDistance<int>();

	public WaterInputPipeDepthFragment(VisualElementLoader visualElementLoader, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		string elementName = "Game/EntityPanel/WaterInputDepthFragment";
		_root = _visualElementLoader.LoadVisualElement(elementName);
		_root.ToggleDisplayStyle(visible: false);
		_depth = _root.Q<Label>("Depth");
		_limit = _root.Q<Label>("Limit");
		_increaseDepth = _root.Q<Button>("IncreaseDepth");
		_increaseDepth.RegisterCallback<ClickEvent>(IncreaseDepth);
		_decreaseDepth = _root.Q<Button>("DecreaseDepth");
		_decreaseDepth.RegisterCallback<ClickEvent>(DecreaseDepth);
		_useDepthLimit = _root.Q<Toggle>("UseDepthLimit");
		_useDepthLimit.RegisterCallback<ClickEvent>(ToggleDepthLimit);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_waterInputPipeSpec = entity.GetComponent<WaterInputPipeSpec>();
		if (_waterInputPipeSpec != null)
		{
			_waterInputPipeCoordinates = entity.GetComponent<WaterInputPipeCoordinates>();
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_waterInputPipeCoordinates = null;
		_waterInputPipeSpec = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if (_waterInputPipeSpec != null)
		{
			bool useDepthLimit = _waterInputPipeCoordinates.UseDepthLimit;
			int depthLimit = _waterInputPipeCoordinates.DepthLimit;
			_increaseDepth.SetEnabled(useDepthLimit && depthLimit < _waterInputPipeSpec.MaxDepth);
			_decreaseDepth.SetEnabled(useDepthLimit && depthLimit > 0);
			_limit.SetEnabled(useDepthLimit);
			_limit.text = _loc.T(_limitPhrase, _waterInputPipeCoordinates.DepthLimit);
			_depth.text = _loc.T(_depthPhrase, _waterInputPipeCoordinates.Depth);
			_useDepthLimit.SetValueWithoutNotify(useDepthLimit);
		}
	}

	private void IncreaseDepth(ClickEvent evt)
	{
		int depthLimit = Math.Min(_waterInputPipeSpec.MaxDepth, _waterInputPipeCoordinates.DepthLimit + 1);
		_waterInputPipeCoordinates.SetDepthLimit(depthLimit);
	}

	private void DecreaseDepth(ClickEvent evt)
	{
		int depthLimit = Math.Max(0, _waterInputPipeCoordinates.DepthLimit - 1);
		_waterInputPipeCoordinates.SetDepthLimit(depthLimit);
	}

	private void ToggleDepthLimit(ClickEvent evt)
	{
		if (_waterInputPipeCoordinates.UseDepthLimit)
		{
			_waterInputPipeCoordinates.DisableDepthLimit();
		}
		else
		{
			_waterInputPipeCoordinates.SetDepthLimit(_waterInputPipeCoordinates.Depth);
		}
	}
}
