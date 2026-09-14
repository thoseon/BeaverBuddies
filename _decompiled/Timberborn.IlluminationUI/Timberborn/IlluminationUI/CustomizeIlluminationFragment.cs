using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.EntityPanelSystem;
using Timberborn.Illumination;
using Timberborn.Localization;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.IlluminationUI;

internal class CustomizeIlluminationFragment : IEntityPanelFragment
{
	private static readonly string CustomizeLocKey = "EntityPanel.Customize";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly ILoc _loc;

	private Button _root;

	private CustomizableIlluminator _customizableIlluminator;

	public CustomizeIlluminationFragment(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = (Button)_visualElementLoader.LoadVisualElement("Common/EntityPanel/CustomizeIlluminationFragment");
		_root.RegisterCallback<ClickEvent>(OnClicked);
		_tooltipRegistrar.Register(_root, _loc.T(CustomizeLocKey));
		_root.ToggleDisplayStyle(visible: false);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_customizableIlluminator = entity.GetComponent<CustomizableIlluminator>();
	}

	public void ClearFragment()
	{
		_customizableIlluminator = null;
		_root.ToggleDisplayStyle(visible: false);
	}

	public void UpdateFragment()
	{
		_root.ToggleDisplayStyle((bool)_customizableIlluminator && !_customizableIlluminator.IsLocked);
	}

	private void OnClicked(ClickEvent evt)
	{
		_customizableIlluminator.SetIsCustomized(!_customizableIlluminator.IsCustomized);
	}
}
