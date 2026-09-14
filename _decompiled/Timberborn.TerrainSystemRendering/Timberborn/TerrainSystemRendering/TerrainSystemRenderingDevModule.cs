using Timberborn.Debugging;

namespace Timberborn.TerrainSystemRendering;

internal class TerrainSystemRenderingDevModule : IDevModule
{
	private readonly TerrainMeshManager _terrainMeshManager;

	public TerrainSystemRenderingDevModule(TerrainMeshManager terrainMeshManager)
	{
		_terrainMeshManager = terrainMeshManager;
	}

	public DevModuleDefinition GetDefinition()
	{
		return new DevModuleDefinition.Builder().AddMethod(DevMethod.Create("Toggle models: Terrain", _terrainMeshManager.ToggleVisibilityForDebugging)).Build();
	}
}
