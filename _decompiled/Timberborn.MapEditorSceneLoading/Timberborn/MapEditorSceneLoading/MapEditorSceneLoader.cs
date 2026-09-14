using Timberborn.BlueprintSystem;
using Timberborn.Common;
using Timberborn.Localization;
using Timberborn.MapRepositorySystem;
using Timberborn.SceneLoading;
using UnityEngine;

namespace Timberborn.MapEditorSceneLoading;

public class MapEditorSceneLoader
{
	private readonly ISceneLoader _sceneLoader;

	private readonly ILoc _loc;

	private readonly ISpecService _specService;

	private readonly IRandomNumberGenerator _randomNumberGenerator;

	public MapEditorSceneLoader(ISceneLoader sceneLoader, ILoc loc, ISpecService specService, IRandomNumberGenerator randomNumberGenerator)
	{
		_sceneLoader = sceneLoader;
		_loc = loc;
		_specService = specService;
		_randomNumberGenerator = randomNumberGenerator;
	}

	public void StartNewMap(Vector2Int mapSize)
	{
		_sceneLoader.LoadScene(MapEditorSceneParameters.CreateNewMapParameters(mapSize), Tip());
	}

	public void StartNewMapInstantly(Vector2Int mapSize)
	{
		_sceneLoader.LoadSceneInstantly(MapEditorSceneParameters.CreateNewMapParameters(mapSize), Tip());
	}

	public void LoadMap(MapFileReference mapFileReference)
	{
		_sceneLoader.LoadScene(MapEditorSceneParameters.CreateExistingMapParameters(mapFileReference), Tip());
	}

	public void LoadMapInstantly(MapFileReference mapFileReference)
	{
		_sceneLoader.LoadSceneInstantly(MapEditorSceneParameters.CreateExistingMapParameters(mapFileReference), Tip());
	}

	private string Tip()
	{
		MapEditorTipSpec singleSpec = _specService.GetSingleSpec<MapEditorTipSpec>();
		string listElement = _randomNumberGenerator.GetListElement(singleSpec.Tips);
		return _loc.T(listElement);
	}
}
