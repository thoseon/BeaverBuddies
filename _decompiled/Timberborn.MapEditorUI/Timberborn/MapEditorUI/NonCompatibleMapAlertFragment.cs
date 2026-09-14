using System;
using Timberborn.AlertPanelSystem;
using Timberborn.CoreUI;
using Timberborn.Localization;
using Timberborn.MapEditorPersistenceUI;
using Timberborn.SingletonSystem;
using Timberborn.TooltipSystem;
using Timberborn.Versioning;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorUI;

internal class NonCompatibleMapAlertFragment : IAlertFragment
{
	private static readonly string LabelLocKey = "MapEditor.NonCompatibleMapVersion";

	private static readonly string TooltipLocKey = "MapEditor.NonCompatibleMapVersion.Tooltip";

	private readonly ILoc _loc;

	private readonly AlertPanelRowFactory _alertPanelRowFactory;

	private readonly MapPersistenceController _mapPersistenceController;

	private readonly ITooltipRegistrar _tooltipRegistrar;

	private readonly EventBus _eventBus;

	private VisualElement _root;

	public NonCompatibleMapAlertFragment(ILoc loc, AlertPanelRowFactory alertPanelRowFactory, MapPersistenceController mapPersistenceController, ITooltipRegistrar tooltipRegistrar, EventBus eventBus)
	{
		_loc = loc;
		_alertPanelRowFactory = alertPanelRowFactory;
		_mapPersistenceController = mapPersistenceController;
		_tooltipRegistrar = tooltipRegistrar;
		_eventBus = eventBus;
	}

	public void InitializeAlertFragment(VisualElement root)
	{
		_root = _alertPanelRowFactory.Create(LabelLocKey, "NonCompatibleVersion");
		_tooltipRegistrar.Register(_root, (Func<string>)GetTooltip);
		_eventBus.Register(this);
		UpdateVisibility();
		root.Add(_root);
	}

	public void UpdateAlertFragment()
	{
	}

	[OnEvent]
	public void OnMapSaved(MapSavedEvent mapSavedEvent)
	{
		UpdateVisibility();
	}

	private string GetTooltip()
	{
		return _loc.T(TooltipLocKey, _mapPersistenceController.CurrentMapVersion, GameVersions.CurrentVersion);
	}

	private void UpdateVisibility()
	{
		_root.ToggleDisplayStyle(!_mapPersistenceController.IsCurrentMapCompatible);
	}
}
