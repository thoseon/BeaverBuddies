using Timberborn.LevelVisibilitySystem;
using Timberborn.MapStateSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.TextureOperations;
using UnityEngine;

namespace Timberborn.TerrainSystemRendering;

internal class TerrainLayerSliceUpdater : ILoadableSingleton, IUnloadableSingleton
{
	private static readonly int TerrainSliceMap = Shader.PropertyToID("_TerrainSliceMap");

	private readonly ITerrainService _terrainService;

	private readonly ILevelVisibilityService _levelVisibilityService;

	private readonly TextureFactory _textureFactory;

	private readonly MapSize _mapSize;

	private Texture2D _terrainSliceMap;

	public TerrainLayerSliceUpdater(ITerrainService terrainService, ILevelVisibilityService levelVisibilityService, TextureFactory textureFactory, MapSize mapSize)
	{
		_terrainService = terrainService;
		_levelVisibilityService = levelVisibilityService;
		_textureFactory = textureFactory;
		_mapSize = mapSize;
	}

	public void Load()
	{
		TextureSettings.Builder builder = new TextureSettings.Builder();
		builder.SetSize(_mapSize.TerrainSize.x, _mapSize.TerrainSize.y).SetTextureFormat(TextureFormat.R8).SetGenerateMipmap(generateMipmap: false);
		_terrainSliceMap = _textureFactory.CreateTexture(builder.Build());
		Shader.SetGlobalTexture(TerrainSliceMap, _terrainSliceMap);
		_terrainService.TerrainHeightChanged += delegate(object _, TerrainHeightChangeEventArgs args)
		{
			OnTerrainHeightChanged(args.Change);
		};
		_levelVisibilityService.MaxVisibleLevelChanged += delegate(object _, int maxVisibleLevel)
		{
			UpdateTerrainSliceTexture(maxVisibleLevel);
		};
	}

	public void Unload()
	{
		Object.Destroy(_terrainSliceMap);
	}

	private void OnTerrainHeightChanged(in TerrainHeightChange change)
	{
		int maxVisibleLevel = _levelVisibilityService.MaxVisibleLevel;
		if (maxVisibleLevel >= change.From && maxVisibleLevel <= change.To)
		{
			UpdateTerrainSliceTexture(maxVisibleLevel);
		}
	}

	private void UpdateTerrainSliceTexture(int maxVisibleLevel)
	{
		for (int i = 0; i < _mapSize.TerrainSize.y; i++)
		{
			for (int j = 0; j < _mapSize.TerrainSize.x; j++)
			{
				bool flag = _terrainService.Underground(new Vector3Int(j, i, maxVisibleLevel));
				_terrainSliceMap.SetPixel(j, i, flag ? Color.white : Color.black);
			}
		}
		_terrainSliceMap.Apply();
	}
}
