using System;
using Timberborn.CoreUI;
using Timberborn.MapRepositorySystemUI;

namespace Timberborn.MapEditorPersistenceUI;

public class MapSaverLoader
{
	private readonly MapPersistenceController _mapPersistenceController;

	private readonly PanelStack _panelStack;

	private readonly SaveMapBox _saveMapBox;

	private readonly LoadMapBox _loadMapBox;

	private readonly NewMapBox _newMapBox;

	public MapSaverLoader(MapPersistenceController mapPersistenceController, PanelStack panelStack, SaveMapBox saveMapBox, LoadMapBox loadMapBox, NewMapBox newMapBox)
	{
		_mapPersistenceController = mapPersistenceController;
		_panelStack = panelStack;
		_saveMapBox = saveMapBox;
		_loadMapBox = loadMapBox;
		_newMapBox = newMapBox;
	}

	public void Save(Action successAction = null)
	{
		if (!_mapPersistenceController.TrySaveCurrent(successAction))
		{
			SaveAs(successAction);
		}
	}

	public void SaveCurrentSilently()
	{
		_mapPersistenceController.SaveCurrentSilently();
	}

	public void SaveAs(Action successAction = null)
	{
		_saveMapBox.Open(successAction);
	}

	public void LoadMap()
	{
		_loadMapBox.OpenAsOverlay();
	}

	public void NewMap()
	{
		_panelStack.HideAndPushOverlay(_newMapBox);
	}
}
