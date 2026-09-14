using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Localization;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorWaterSourceSystemUI;

internal class WaterSourceFlowPreviewFragment : IEntityPanelFragment
{
	private static readonly string EnableFlowLocKey = "MapEditor.FlowPreview.StartTest";

	private static readonly string DisableFlowLocKey = "MapEditor.FlowPreview.StopTest";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ILoc _loc;

	private WaterSourceFlowPreview _waterSourceFlowPreview;

	private VisualElement _root;

	private Button _button;

	public WaterSourceFlowPreviewFragment(VisualElementLoader visualElementLoader, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = _visualElementLoader.LoadVisualElement("MapEditor/EntityPanel/WaterSourceFlowPreviewFragment");
		_button = _root.Q<Button>("Button");
		_button.RegisterCallback<ClickEvent>(ToggleForced);
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		WaterSourceFlowPreview component = entity.GetComponent<WaterSourceFlowPreview>();
		if (component != null)
		{
			_waterSourceFlowPreview = component;
			_root.ToggleDisplayStyle(visible: true);
		}
	}

	public void ClearFragment()
	{
		_waterSourceFlowPreview = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		if ((bool)_waterSourceFlowPreview)
		{
			_root.ToggleDisplayStyle(_waterSourceFlowPreview.CanEnable);
			_button.text = (_waterSourceFlowPreview.IsEnabled ? _loc.T(DisableFlowLocKey) : _loc.T(EnableFlowLocKey));
		}
	}

	private void ToggleForced(ClickEvent evt)
	{
		if (_waterSourceFlowPreview.IsEnabled)
		{
			_waterSourceFlowPreview.DisableFlowPreview();
		}
		else
		{
			_waterSourceFlowPreview.EnableFlowPreview();
		}
	}
}
