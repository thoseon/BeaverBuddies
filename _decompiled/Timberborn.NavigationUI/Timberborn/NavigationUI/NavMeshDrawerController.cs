using Timberborn.CursorToolSystem;
using Timberborn.Debugging;
using Timberborn.Navigation;
using Timberborn.SingletonSystem;

namespace Timberborn.NavigationUI;

public class NavMeshDrawerController : IDevModule, IUpdatableSingleton
{
	private readonly INavMeshDrawer _navMeshDrawer;

	private readonly CursorCoordinatesPicker _cursorCoordinatesPicker;

	private bool _draw;

	public NavMeshDrawerController(INavMeshDrawer navMeshDrawer, CursorCoordinatesPicker cursorCoordinatesPicker)
	{
		_navMeshDrawer = navMeshDrawer;
		_cursorCoordinatesPicker = cursorCoordinatesPicker;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Toggle nav mesh", Toggle)).Build();
	}

	public void UpdateSingleton()
	{
		if (_draw)
		{
			DrawNavMesh();
		}
	}

	private void Toggle()
	{
		_draw = !_draw;
	}

	private void DrawNavMesh()
	{
		CursorCoordinates? cursorCoordinates = _cursorCoordinatesPicker.Pick();
		if (cursorCoordinates.HasValue)
		{
			CursorCoordinates valueOrDefault = cursorCoordinates.GetValueOrDefault();
			_navMeshDrawer.DrawForOneFrameAroundCoordinates(valueOrDefault.TileCoordinates);
		}
	}
}
