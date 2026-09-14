using System;
using System.Collections.Generic;
using Timberborn.Common;
using Timberborn.MapEditorTickSystem;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;
using Timberborn.TickSystem;
using Timberborn.WaterSystem;
using UnityEngine;

namespace Timberborn.WaterSourceRendering;

[MapEditorTickable]
internal class WaterSourceRenderingService : ILoadableSingleton, IPostLoadableSingleton, IUnloadableSingleton, ITickableSingleton, ILateUpdatableSingleton
{
	private static readonly int TextureId = Shader.PropertyToID("_WaterSourceMask");

	private static readonly int TwoByTwoUVOffset = 0;

	private static readonly int ThreeByThreeUVOffset = 4;

	private readonly MapIndexService _mapIndexService;

	private readonly IThreadSafeWaterMap _threadSafeWaterMap;

	private readonly Dictionary<WaterSourceRenderer, RendererMaskItem> _maskItems = new Dictionary<WaterSourceRenderer, RendererMaskItem>();

	private byte[][] _renderersMask;

	private Texture2DArray _renderersMaskTexture;

	private Vector3Int _mapSize;

	private int _verticalStride;

	private bool _isInitialized;

	private bool _maskIsDirty;

	private bool _fullUpdateScheduled;

	public WaterSourceRenderingService(MapIndexService mapIndexService, IThreadSafeWaterMap threadSafeWaterMap)
	{
		_mapIndexService = mapIndexService;
		_threadSafeWaterMap = threadSafeWaterMap;
	}

	public void Load()
	{
		_mapSize = _mapIndexService.TerrainSize;
		_verticalStride = _mapIndexService.VerticalStride;
		_threadSafeWaterMap.MaxWaterColumnCountChanged += OnMaxColumnCountChanged;
	}

	public void PostLoad()
	{
		_isInitialized = true;
		UpdateAll();
	}

	public void Unload()
	{
		Cleanup();
	}

	public void Tick()
	{
		if (_fullUpdateScheduled)
		{
			UpdateAll();
			_fullUpdateScheduled = false;
		}
		else if (ComputeMaskVisibilityChanges())
		{
			UpdateMask();
		}
	}

	public void LateUpdateSingleton()
	{
		if (_maskIsDirty)
		{
			ApplyMaskToTexture();
			_maskIsDirty = false;
		}
	}

	public void AddRenderer(WaterSourceRenderer waterSourceRenderer)
	{
		_maskItems[waterSourceRenderer] = new RendererMaskItem(waterSourceRenderer);
		ScheduleFullUpdate();
	}

	public void RemoveRenderer(WaterSourceRenderer waterSourceRenderer)
	{
		_maskItems.Remove(waterSourceRenderer);
		ScheduleFullUpdate();
	}

	private void OnMaxColumnCountChanged(object sender, int columnCount)
	{
		SetupMaskResources(columnCount);
		ScheduleFullUpdate();
	}

	private void SetupMaskResources(int columnCount)
	{
		Cleanup();
		Array.Resize(ref _renderersMask, columnCount);
		int num = _mapSize.x * _mapSize.y;
		for (int i = 0; i < columnCount; i++)
		{
			_renderersMask[i] = new byte[num];
		}
		_renderersMaskTexture = new Texture2DArray(_mapSize.x, _mapSize.y, columnCount, TextureFormat.R8, mipChain: false)
		{
			filterMode = FilterMode.Point,
			wrapMode = TextureWrapMode.Clamp
		};
		Shader.SetGlobalTexture(TextureId, _renderersMaskTexture);
	}

	private void Cleanup()
	{
		if (_renderersMaskTexture != null)
		{
			UnityEngine.Object.Destroy(_renderersMaskTexture);
			_renderersMaskTexture = null;
		}
	}

	private void ScheduleFullUpdate()
	{
		_fullUpdateScheduled = true;
	}

	private void UpdateAll()
	{
		ComputeMaskVisibilityChanges();
		UpdateMask();
	}

	private bool ComputeMaskVisibilityChanges()
	{
		if (_isInitialized)
		{
			bool result = false;
			{
				foreach (RendererMaskItem value in _maskItems.Values)
				{
					bool hasFullyVisibleWaterSurfaceAbove = HasFullyVisibleWaterSurfaceAbove(value);
					if (value.UpdateVisibility(hasFullyVisibleWaterSurfaceAbove))
					{
						result = true;
					}
				}
				return result;
			}
		}
		return false;
	}

	private bool HasFullyVisibleWaterSurfaceAbove(RendererMaskItem maskItem)
	{
		foreach (Vector3Int coordinate in maskItem.Coordinates)
		{
			int columnIndex = GetColumnIndex(coordinate);
			int index3D = _mapIndexService.CellToIndex(coordinate.XY()) + columnIndex * _verticalStride;
			float num = _threadSafeWaterMap.WaterDepth(index3D);
			byte num2 = _threadSafeWaterMap.ColumnFloor(index3D);
			byte b = _threadSafeWaterMap.ColumnCeiling(index3D);
			if ((float)(int)num2 + num >= (float)(int)b)
			{
				return false;
			}
		}
		return true;
	}

	private int GetColumnIndex(Vector3Int coordinates)
	{
		int num = _mapIndexService.CellToIndex(coordinates.XY());
		int num2 = _threadSafeWaterMap.ColumnCount(num);
		for (int i = 0; i < num2; i++)
		{
			int index3D = num + i * _verticalStride;
			if (_threadSafeWaterMap.ColumnFloor(index3D) == coordinates.z)
			{
				return i;
			}
		}
		throw new InvalidOperationException($"Column at {coordinates} not found.");
	}

	private void UpdateMask()
	{
		if (!_isInitialized)
		{
			return;
		}
		ClearMask();
		foreach (RendererMaskItem value in _maskItems.Values)
		{
			int num = GetMaskInitialOffset(value);
			foreach (Vector3Int coordinate in value.Coordinates)
			{
				int columnIndex = GetColumnIndex(coordinate);
				int num2 = _mapIndexService.CoordinatesToActualMapIndex(coordinate.XY());
				_renderersMask[columnIndex][num2] = (byte)(value.IsVisible ? ((uint)(++num)) : 0u);
			}
		}
		_maskIsDirty = true;
	}

	private void ClearMask()
	{
		int num = _renderersMask.Length;
		for (int i = 0; i < num; i++)
		{
			byte[] array = _renderersMask[i];
			Array.Clear(array, 0, array.Length);
		}
	}

	private void ApplyMaskToTexture()
	{
		for (int i = 0; i < _renderersMask.Length; i++)
		{
			_renderersMaskTexture.SetPixelData(_renderersMask[i], 0, i);
		}
		_renderersMaskTexture.Apply(updateMipmaps: false, makeNoLongerReadable: false);
	}

	private static int GetMaskInitialOffset(RendererMaskItem maskItem)
	{
		int length = maskItem.Coordinates.Length;
		return length switch
		{
			4 => TwoByTwoUVOffset, 
			9 => ThreeByThreeUVOffset, 
			_ => throw new NotSupportedException($"Source with {length} tiles is not supported."), 
		};
	}
}
