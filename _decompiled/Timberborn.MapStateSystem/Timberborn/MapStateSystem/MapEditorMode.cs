namespace Timberborn.MapStateSystem;

public class MapEditorMode
{
	public bool IsMapEditor { get; }

	private MapEditorMode(bool isMapEditor)
	{
		IsMapEditor = isMapEditor;
	}

	public static MapEditorMode MapEditorInstance()
	{
		return new MapEditorMode(isMapEditor: true);
	}

	public static MapEditorMode NonMapEditorInstance()
	{
		return new MapEditorMode(isMapEditor: false);
	}
}
