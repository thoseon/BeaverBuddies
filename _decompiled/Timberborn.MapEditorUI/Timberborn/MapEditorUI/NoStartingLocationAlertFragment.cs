using Timberborn.AlertPanelSystem;
using Timberborn.CoreUI;
using Timberborn.StartingLocationSystem;
using UnityEngine.UIElements;

namespace Timberborn.MapEditorUI;

internal class NoStartingLocationAlertFragment : IAlertFragment
{
	private static readonly string LabelLocKey = "MapEditor.NoStartingLocation";

	private readonly AlertPanelRowFactory _alertPanelRowFactory;

	private readonly StartingLocationService _startingLocationService;

	private VisualElement _root;

	public NoStartingLocationAlertFragment(AlertPanelRowFactory alertPanelRowFactory, StartingLocationService startingLocationService)
	{
		_alertPanelRowFactory = alertPanelRowFactory;
		_startingLocationService = startingLocationService;
	}

	public void InitializeAlertFragment(VisualElement root)
	{
		_root = _alertPanelRowFactory.Create(LabelLocKey, "NoStartingLocation");
		root.Add(_root);
	}

	public void UpdateAlertFragment()
	{
		_root.ToggleDisplayStyle(!_startingLocationService.HasStartingLocation());
	}
}
