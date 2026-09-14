using System;
using System.Diagnostics;
using Timberborn.Common;
using Timberborn.MapEditorTickSystem;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;
using Timberborn.TerrainSystem;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WaterSystemRendering;

[MapEditorTickable]
internal class WaterRenderer : IWaterRenderer, ILoadableSingleton, IPostLoadableSingleton, IUnloadableSingleton, ITickableSingleton, IParallelTickableSingleton
{
	private static readonly int MapSizeProperty = Shader.PropertyToID("_MapSize");

	private readonly MapIndexService _mapIndexService;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly WaterColumnPostprocessor _waterColumnPostprocessor;

	private readonly IWaterMesh _waterMesh;

	private readonly IFlowLimiterService _flowLimiterService;

	private readonly WaterRenderingTaskStarter _waterRenderingTaskStarter;

	private readonly WaterFlowLimitUpdater _waterFlowLimitUpdater;

	private DataTextureArray<float> _waterDepths;

	private DataTextureArray<Vector2> _outflows;

	private DataTextureArray<byte> _contaminations;

	private DataTextureArray<Vector2> _columns;

	private DataTextureArray<Vector2> _linkBarriers;

	private DataTextureArray<float> _flowLimits;

	private bool[] _tilesWithWater;

	private readonly ColumnCountTracker _columnCountTracker = new ColumnCountTracker();

	private readonly ColumnChangeTracker _columnChangeTracker = new ColumnChangeTracker();

	private Vector2Int _mapSize;

	private Vector2Int _tileCount;

	private bool _updateMeshes = true;

	private bool _updateTextures = true;

	private bool _postprocessEnabled = true;

	private readonly Stopwatch _stopwatch = new Stopwatch();

	private bool _shouldResize;

	public long UpdateMeshTime { get; private set; }

	public long UpdateTexturesTime { get; private set; }

	public WaterRenderer(MapIndexService mapIndexService, IThreadSafeWaterMap threadSafeWaterMap, WaterColumnPostprocessor waterColumnPostprocessor, IWaterMesh waterMesh, IFlowLimiterService flowLimiterService, WaterRenderingTaskStarter waterRenderingTaskStarter, WaterFlowLimitUpdater waterFlowLimitUpdater)
	{
		_mapIndexService = mapIndexService;
		_threadSafeWaterMap = threadSafeWaterMap;
		_waterColumnPostprocessor = waterColumnPostprocessor;
		_waterMesh = waterMesh;
		_flowLimiterService = flowLimiterService;
		_waterRenderingTaskStarter = waterRenderingTaskStarter;
		_waterFlowLimitUpdater = waterFlowLimitUpdater;
	}

	public void Load()
	{
		_mapSize = _mapIndexService.TerrainSize.XY();
		_threadSafeWaterMap.MaxWaterColumnCountChanged += delegate
		{
			_shouldResize = true;
		};
		_tileCount = WorldTiling.TileCount2D(_mapSize.x, _mapSize.y);
		_tilesWithWater = new bool[_tileCount.x * _tileCount.y];
		CreateDataTextureArrays();
		Shader.SetGlobalVector(MapSizeProperty, new Vector2(_mapSize.x, _mapSize.y));
	}

	public void PostLoad()
	{
		Resize();
		_waterFlowLimitUpdater.UpdateFlowLimits();
		FullyUpdateWater();
	}

	public void Unload()
	{
		_waterDepths.Cleanup();
		_outflows.Cleanup();
		_contaminations.Cleanup();
		_columns.Cleanup();
		_linkBarriers.Cleanup();
		_flowLimits.Cleanup();
	}

	public void Tick()
	{
		if (_shouldResize)
		{
			Resize();
			FullyUpdateWater();
			_shouldResize = false;
		}
		_waterFlowLimitUpdater.UpdateFlowLimits();
		UpdateMeshAndTextures();
	}

	public void StartParallelTick()
	{
		StartWaterRenderingTask(waitForResult: false);
	}

	public void EnableMeshUpdate()
	{
		_updateMeshes = true;
	}

	public void DisableMeshUpdate()
	{
		_updateMeshes = false;
	}

	public void DisableTextureUpdate()
	{
		_updateTextures = false;
	}

	public void EnableTextureUpdate()
	{
		_updateTextures = true;
	}

	public void DisablePostprocessing()
	{
		_postprocessEnabled = false;
	}

	public void EnablePostprocessing()
	{
		_postprocessEnabled = true;
	}

	private void Resize()
	{
		int maxColumnCount = _threadSafeWaterMap.MaxColumnCount;
		_columnCountTracker.Update(maxColumnCount);
		_waterDepths.Resize(maxColumnCount);
		_outflows.Resize(maxColumnCount);
		_contaminations.Resize(maxColumnCount);
		_columns.Resize(maxColumnCount);
		_linkBarriers.Resize(maxColumnCount);
		_flowLimits.Resize(maxColumnCount);
		_waterColumnPostprocessor.Resize(maxColumnCount);
		_waterFlowLimitUpdater.Resize(maxColumnCount);
		Array.Resize(ref _tilesWithWater, _tileCount.x * _tileCount.y * maxColumnCount);
	}

	private void FullyUpdateWater()
	{
		StartWaterRenderingTask(waitForResult: true);
		StartWaterRenderingTask(waitForResult: true);
		UpdateMeshAndTextures();
		UpdateMeshAndTextures();
	}

	private void CreateDataTextureArrays()
	{
		_waterDepths = DataTextureArray<float>.Create(TextureFormat.RFloat, _mapSize);
		_outflows = DataTextureArray<Vector2>.Create(TextureFormat.RGFloat, _mapSize);
		_contaminations = DataTextureArray<byte>.Create(TextureFormat.R8, _mapSize);
		_columns = DataTextureArray<Vector2>.Create(TextureFormat.RGFloat, _mapSize);
		_linkBarriers = DataTextureArray<Vector2>.Create(TextureFormat.RGFloat, _mapSize);
		_flowLimits = DataTextureArray<float>.Create(TextureFormat.RFloat, _mapSize);
	}

	private void UpdateMeshAndTextures()
	{
		bool flag = _columnChangeTracker.AnyColumnChanged();
		_stopwatch.Restart();
		if (_updateMeshes)
		{
			_waterMesh.DisableAllTiles();
			for (int i = 0; i < _tilesWithWater.Length; i++)
			{
				if (_tilesWithWater[i])
				{
					Vector3Int tileIndex = WorldTiling.TileIndex3DToCoordinates(i, _tileCount.x, _tileCount.y);
					_waterMesh.EnableTile(tileIndex);
				}
			}
			UpdateMeshTime = _stopwatch.ElapsedMilliseconds;
		}
		_stopwatch.Restart();
		if (_updateTextures)
		{
			_waterDepths.SwapTextureArrays();
			_outflows.SwapTextureArrays();
			_contaminations.SwapTextureArrays();
			_flowLimits.SwapTextureArrays();
			if (flag)
			{
				_columns.SwapTextureArrays();
				_linkBarriers.SwapTextureArrays();
			}
			for (int j = 0; j < _columnCountTracker.MaxCount; j++)
			{
				_waterDepths.UpdateTextureArrays(j);
				_outflows.UpdateTextureArrays(j);
				_contaminations.UpdateTextureArrays(j);
				_flowLimits.UpdateTextureArrays(j);
				if (flag)
				{
					_columns.UpdateTextureArrays(j);
					_linkBarriers.UpdateTextureArrays(j);
				}
			}
		}
		UpdateTexturesTime = _stopwatch.ElapsedMilliseconds;
		if (_postprocessEnabled)
		{
			_waterColumnPostprocessor.Postprocess(_columnCountTracker.MaxCount, _waterDepths, _columns, _outflows, _contaminations, _linkBarriers, _flowLimits);
		}
	}

	private void StartWaterRenderingTask(bool waitForResult)
	{
		_waterRenderingTaskStarter.StartTask(waitForResult, _threadSafeWaterMap.MaxColumnCount, _threadSafeWaterMap.AnyColumnChanged, _columnChangeTracker, _columnCountTracker, _waterDepths, _outflows, _contaminations, _columns, _linkBarriers, _flowLimits, _tilesWithWater, _waterFlowLimitUpdater.FlowLimits, _threadSafeWaterMap.ColumnCounts, _threadSafeWaterMap.WaterColumns, _threadSafeWaterMap.FlowDirections, _flowLimiterService.LimitedDirections);
	}
}
