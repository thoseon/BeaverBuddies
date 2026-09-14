using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.UndoSystem;

namespace Timberborn.TerrainUndoSystem;

internal class TerrainUndoableRegistrar : ILoadableSingleton
{
	private readonly IUndoRegistry _undoRegistry;

	private readonly ITerrainService _terrainService;

	public TerrainUndoableRegistrar(IUndoRegistry undoRegistry, ITerrainService terrainService)
	{
		_undoRegistry = undoRegistry;
		_terrainService = terrainService;
	}

	public void Load()
	{
		_terrainService.TerrainHeightChanged += OnTerrainChanged;
	}

	private void OnTerrainChanged(object sender, TerrainHeightChangeEventArgs terrainHeightChangeEventArgs)
	{
		if (_undoRegistry.UndoAllowed && !_undoRegistry.IsProcessingStack)
		{
			TerrainHeightChange change = terrainHeightChangeEventArgs.Change;
			UndoableTerrain undoableTerrain = new UndoableTerrain(_terrainService, change);
			IUndoable undoable2;
			if (!change.SetTerrain)
			{
				IUndoable undoable = new DeletedTerrainUndoable(undoableTerrain);
				undoable2 = undoable;
			}
			else
			{
				IUndoable undoable = new CreatedTerrainUndoable(undoableTerrain);
				undoable2 = undoable;
			}
			IUndoable undoable3 = undoable2;
			_undoRegistry.RegisterStackedUndoable(undoable3);
		}
	}
}
