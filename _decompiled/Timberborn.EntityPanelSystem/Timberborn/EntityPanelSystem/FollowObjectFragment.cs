using Timberborn.BaseComponentSystem;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.SelectionSystem;
using Timberborn.TooltipSystem;
using UnityEngine.UIElements;

namespace Timberborn.EntityPanelSystem;

internal class FollowObjectFragment : IEntityPanelFragment
{
	private static readonly string FocusLocKey = "EntityPanel.Focus";

	private readonly VisualElementLoader _visualElementLoader;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly AlternateClickableFactory _alternateClickableFactory;

	private readonly EntitySelectionService _entitySelectionService;

	private readonly ILoc _loc;

	private Button _root;

	private AlternateClickable _alternateClickable;

	private BaseComponent _shownEntity;

	public FollowObjectFragment(VisualElementLoader visualElementLoader, ITooltipRegistrar tooltipRegistrar, AlternateClickableFactory alternateClickableFactory, EntitySelectionService entitySelectionService, ILoc loc)
	{
		_visualElementLoader = visualElementLoader;
		_tooltipRegistrar = tooltipRegistrar;
		_alternateClickableFactory = alternateClickableFactory;
		_entitySelectionService = entitySelectionService;
		_loc = loc;
	}

	public VisualElement InitializeFragment()
	{
		_root = (Button)_visualElementLoader.LoadVisualElement("Common/EntityPanel/FollowObjectFragment");
		_tooltipRegistrar.Register(_root, _loc.T(FocusLocKey));
		_alternateClickable = _alternateClickableFactory.Create(_root, SelectAndFollow, UnselectAndFollow);
		return _root;
	}

	public void ShowFragment(BaseComponent entity)
	{
		_shownEntity = entity;
	}

	public void ClearFragment()
	{
		_shownEntity = null;
	}

	public void UpdateFragment()
	{
		_alternateClickable.Update();
	}

	private void SelectAndFollow()
	{
		_entitySelectionService.SelectAndFollow(_shownEntity);
	}

	private void UnselectAndFollow()
	{
		_entitySelectionService.UnselectAndFollow(_shownEntity);
	}
}
