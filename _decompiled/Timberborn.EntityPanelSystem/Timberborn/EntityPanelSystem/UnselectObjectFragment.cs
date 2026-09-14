using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.EntityPanelSystem;

internal class UnselectObjectFragment : IEntityPanelFragment
{
	private static readonly string CloseLocKey = "EntityPanel.Close";

	private static readonly string CloseKeyBindingId = "Cancel";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly ILoc _loc;

	private Button _root;

	public UnselectObjectFragment(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, EntitySelectionService entitySelectionService, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_entitySelectionService = entitySelectionService;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = (Button)_visualElementLoader.LoadVisualElement("Common/EntityPanel/UnselectObjectFragment");
		_root.RegisterCallback<ClickEvent>(OnClicked);
		_tooltipRegistrar.RegisterWithKeyBinding(_root, _loc.T(CloseLocKey), CloseKeyBindingId);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
	}

	public void ClearFragment()
	{
	}

	public void UpdateFragment()
	{
	}

	private void OnClicked(ClickEvent clickEvent)
	{
		_entitySelectionService.Unselect();
	}
}
