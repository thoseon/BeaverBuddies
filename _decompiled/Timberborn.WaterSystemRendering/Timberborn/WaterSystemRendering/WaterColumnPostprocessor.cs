using System;
using Timberborn.AssetSystem;
using Timberborn.GraphicsQualitySystem;
using Timberborn.MapIndexSystem;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.Rendering;

namespace Timberborn.WaterSystemRendering;

internal class WaterColumnPostprocessor : ILoadableSingleton, IPostLoadableSingleton, IUnloadableSingleton
{
	private static readonly string PostprocessingShaderPath = "Rendering/WaterColumnPostprocessor";

	private static readonly int MaxIndexProperty = GetId("MaxIndex");

	private static readonly string ProcessColumnsKernel = "ProcessColumns";

	private static readonly string FindBestPropertiesKernel = "FindBestProperties";

	private static readonly string BreakCornerLinksKernel = "BreakCornerLinks";

	private static readonly string BreakCornerLinksReversedKernel = "BreakCornerLinksReversed";

	private static readonly string CalculateHeightsKernel = "CalculateHeights";

	private static readonly string FindWaterfallsKernel = "FindWaterfalls";

	private static readonly int InDepthsId = GetId("InDepths");

	private static readonly int InColumnsId = GetId("InColumns");

	private static readonly int InOutflowsId = GetId("InOutflows");

	private static readonly int InContaminationsId = GetId("InContaminations");

	private static readonly int InLinkBarriersId = GetId("InLinkBarriers");

	private static readonly int InFlowLimitsId = GetId("InFlowLimits");

	private static readonly int BrokenCornerLinksId = GetId("BrokenCornerLinks");

	private static readonly int BrokenCornerLinksBufferId = GetId("BrokenCornerLinksBuffer");

	private static readonly int OutWaterDataId = GetId("OutWaterData");

	private static readonly int OutWaterDataBufferId = GetId("OutWaterDataBuffer");

	private static readonly int OutEdgeLinksId = GetId("OutEdgeLinks");

	private static readonly int OutEdgeLinksBufferId = GetId("OutEdgeLinksBuffer");

	private static readonly int OutCornerLinksId = GetId("OutCornerLinks");

	private static readonly int OutCornerLinksBufferId = GetId("OutCornerLinksBuffer");

	private static readonly int OutOutflowsId = GetId("OutOutflows");

	private static readonly int OutContaminationsId = GetId("OutContaminations");

	private static readonly int OutBaseCornerLinksId = GetId("OutBaseCornerLinks");

	private static readonly int OutSkirtsId = GetId("OutSkirts");

	private static readonly int OutWaterfallsId = GetId("OutWaterfalls");

	private static readonly int OutHeightsId = GetId("OutHeights");

	private static readonly int OutHeightsBufferId = GetId("OutHeightsBuffer");

	private readonly MapIndexService _mapIndexService;

	private readonly IAssetLoader _assetLoader;

	private readonly WaterQualitySetting _waterQualitySetting;

	private ComputeShader _shader;

	private int _processColumnsKernel;

	private int _findBestPropertiesKernel;

	private int _breakCornerLinksKernel;

	private int _breakCornerLinksReversedKernel;

	private int _calculateHeightsKernel;

	private int _findWaterfallsKernel;

	private RenderTexture _oldBrokenCornerLinks;

	private RenderTexture _newBrokenCornerLinks;

	private RenderTexture _oldWaterData;

	private RenderTexture _newWaterData;

	private RenderTexture _oldEdgeLinks;

	private RenderTexture _newEdgeLinks;

	private RenderTexture _oldCornerLinks;

	private RenderTexture _newCornerLinks;

	private RenderTexture _oldBaseCornerLinks;

	private RenderTexture _newBaseCornerLinks;

	private RenderTexture _oldSkirts;

	private RenderTexture _newSkirts;

	private RenderTexture _oldWaterHeights;

	private RenderTexture _newWaterHeights;

	private RenderTexture _oldWaterfalls;

	private RenderTexture _newWaterfalls;

	private RenderTexture _oldOutflows;

	private RenderTexture _newOutflows;

	private RenderTexture _oldContaminations;

	private RenderTexture _newContaminations;

	private RenderTexture _oldCornerLinksBuffer;

	private RenderTexture _newCornerLinksBuffer;

	private Vector2Int _tileThreadGroupCount;

	private Vector2Int _vertexThreadGroupCount;

	private int _arraySize;

	public WaterColumnPostprocessor(MapIndexService mapIndexService, IAssetLoader assetLoader, WaterQualitySetting waterQualitySetting)
	{
		_mapIndexService = mapIndexService;
		_assetLoader = assetLoader;
		_waterQualitySetting = waterQualitySetting;
	}

	public void Load()
	{
		_shader = UnityEngine.Object.Instantiate(_assetLoader.Load<ComputeShader>(PostprocessingShaderPath));
		_processColumnsKernel = _shader.FindKernel(ProcessColumnsKernel);
		_findBestPropertiesKernel = _shader.FindKernel(FindBestPropertiesKernel);
		_breakCornerLinksKernel = _shader.FindKernel(BreakCornerLinksKernel);
		_breakCornerLinksReversedKernel = _shader.FindKernel(BreakCornerLinksReversedKernel);
		_calculateHeightsKernel = _shader.FindKernel(CalculateHeightsKernel);
		_findWaterfallsKernel = _shader.FindKernel(FindWaterfallsKernel);
	}

	public void PostLoad()
	{
		_waterQualitySetting.WaterQualityChanged += OnWaterQualityChanged;
	}

	public void Unload()
	{
		UnloadComputeShader();
		UnloadRenderTextures();
	}

	public void Resize(int arraySize)
	{
		_arraySize = arraySize + 1;
		UnloadRenderTextures();
		InitializeTextures();
	}

	public void Postprocess(int maxIndex, IDataTextureArray depths, IDataTextureArray columns, IDataTextureArray outflows, IDataTextureArray contaminations, IDataTextureArray linkBarriers, IDataTextureArray flowLimits)
	{
		_shader.SetFloat(MaxIndexProperty, maxIndex);
		DispatchAll(depths, columns, outflows, contaminations, linkBarriers, flowLimits);
	}

	private void InitializeTextures()
	{
		Vector3Int tileMapTextureSize = GetTileMapTextureSize();
		_oldBrokenCornerLinks = CreateTexture(tileMapTextureSize, RenderTextureFormat.R8);
		_newBrokenCornerLinks = CreateTexture(tileMapTextureSize, RenderTextureFormat.R8);
		_oldWaterData = CreateTexture(tileMapTextureSize, RenderTextureFormat.ARGBFloat, WaterTextureNames.OldWaterData);
		_newWaterData = CreateTexture(tileMapTextureSize, RenderTextureFormat.ARGBFloat, WaterTextureNames.NewWaterData);
		_oldEdgeLinks = CreateTexture(tileMapTextureSize, RenderTextureFormat.ARGBFloat, WaterTextureNames.OldEdgeLinks);
		_newEdgeLinks = CreateTexture(tileMapTextureSize, RenderTextureFormat.ARGBFloat, WaterTextureNames.NewEdgeLinks);
		_oldCornerLinks = CreateTexture(tileMapTextureSize, RenderTextureFormat.ARGBFloat, WaterTextureNames.OldCornerLinks);
		_newCornerLinks = CreateTexture(tileMapTextureSize, RenderTextureFormat.ARGBFloat, WaterTextureNames.NewCornerLinks);
		_oldBaseCornerLinks = CreateTexture(tileMapTextureSize, RenderTextureFormat.ARGBFloat, WaterTextureNames.OldBaseCornerLinks);
		_newBaseCornerLinks = CreateTexture(tileMapTextureSize, RenderTextureFormat.ARGBFloat, WaterTextureNames.NewBaseCornerLinks);
		_oldSkirts = CreateTexture(tileMapTextureSize, RenderTextureFormat.ARGB32, WaterTextureNames.OldSkirts);
		_newSkirts = CreateTexture(tileMapTextureSize, RenderTextureFormat.ARGB32, WaterTextureNames.NewSkirts);
		_oldOutflows = CreateTexture(tileMapTextureSize, RenderTextureFormat.RGFloat, WaterTextureNames.OldOutflows);
		_newOutflows = CreateTexture(tileMapTextureSize, RenderTextureFormat.RGFloat, WaterTextureNames.NewOutflows);
		_oldContaminations = CreateTexture(tileMapTextureSize, RenderTextureFormat.R8, WaterTextureNames.OldContaminations);
		_newContaminations = CreateTexture(tileMapTextureSize, RenderTextureFormat.R8, WaterTextureNames.NewContaminations);
		_oldCornerLinksBuffer = CreateTexture(tileMapTextureSize, RenderTextureFormat.ARGBFloat);
		_newCornerLinksBuffer = CreateTexture(tileMapTextureSize, RenderTextureFormat.ARGBFloat);
		CreateWaterfallTextures();
		Vector3Int vertexMapTextureSize = GetVertexMapTextureSize();
		_oldWaterHeights = CreateTexture(vertexMapTextureSize, RenderTextureFormat.RFloat, WaterTextureNames.OldWaterHeights);
		_newWaterHeights = CreateTexture(vertexMapTextureSize, RenderTextureFormat.RFloat, WaterTextureNames.NewWaterHeights);
		_shader.GetKernelThreadGroupSizes(_processColumnsKernel, out var x, out var y, out var _);
		_tileThreadGroupCount = new Vector2Int(Mathf.CeilToInt((float)tileMapTextureSize.x / (float)x), Mathf.CeilToInt((float)tileMapTextureSize.y / (float)y));
		_vertexThreadGroupCount = new Vector2Int(Mathf.CeilToInt((float)vertexMapTextureSize.x / (float)x), Mathf.CeilToInt((float)vertexMapTextureSize.y / (float)y));
	}

	private static RenderTexture CreateTexture(Vector3Int size, RenderTextureFormat textureFormat, string propertyName = null)
	{
		RenderTexture renderTexture = new RenderTexture(size.x, size.y, 0, textureFormat)
		{
			dimension = TextureDimension.Tex2DArray,
			enableRandomWrite = true,
			useMipMap = false,
			autoGenerateMips = false,
			anisoLevel = 0,
			antiAliasing = 1,
			useDynamicScale = false,
			volumeDepth = size.z,
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
		if (!string.IsNullOrWhiteSpace(propertyName))
		{
			renderTexture.name = propertyName;
			Shader.SetGlobalTexture(GetId(propertyName), renderTexture);
		}
		return renderTexture;
	}

	private void CreateWaterfallTextures()
	{
		if (_waterQualitySetting.HighQualityWaterEnabled)
		{
			Vector3Int tileMapTextureSize = GetTileMapTextureSize();
			_oldWaterfalls = CreateTexture(tileMapTextureSize, RenderTextureFormat.RFloat, WaterTextureNames.OldWaterfalls);
			_newWaterfalls = CreateTexture(tileMapTextureSize, RenderTextureFormat.RFloat, WaterTextureNames.NewWaterfalls);
		}
	}

	private void UnloadComputeShader()
	{
		if (_shader != null)
		{
			UnityEngine.Object.Destroy(_shader);
			_shader = null;
		}
	}

	private void UnloadRenderTextures()
	{
		UnloadRenderTexture(_oldBrokenCornerLinks);
		UnloadRenderTexture(_newBrokenCornerLinks);
		UnloadRenderTexture(_oldWaterData);
		UnloadRenderTexture(_newWaterData);
		UnloadRenderTexture(_oldEdgeLinks);
		UnloadRenderTexture(_newEdgeLinks);
		UnloadRenderTexture(_oldCornerLinks);
		UnloadRenderTexture(_newCornerLinks);
		UnloadRenderTexture(_oldBaseCornerLinks);
		UnloadRenderTexture(_newBaseCornerLinks);
		UnloadRenderTexture(_oldSkirts);
		UnloadRenderTexture(_newSkirts);
		UnloadRenderTexture(_oldWaterHeights);
		UnloadRenderTexture(_newWaterHeights);
		UnloadRenderTexture(_oldOutflows);
		UnloadRenderTexture(_newOutflows);
		UnloadRenderTexture(_oldContaminations);
		UnloadRenderTexture(_newContaminations);
		UnloadRenderTexture(_oldCornerLinksBuffer);
		UnloadRenderTexture(_newCornerLinksBuffer);
		UnloadWaterfallTextures();
	}

	private void UnloadWaterfallTextures()
	{
		UnloadRenderTexture(_oldWaterfalls);
		UnloadRenderTexture(_newWaterfalls);
	}

	private void OnWaterQualityChanged(object sender, EventArgs e)
	{
		UnloadWaterfallTextures();
		CreateWaterfallTextures();
	}

	private static void UnloadRenderTexture(RenderTexture texture)
	{
		if (texture != null)
		{
			texture.Release();
			UnityEngine.Object.Destroy(texture);
		}
	}

	private void DispatchAll(IDataTextureArray depths, IDataTextureArray columns, IDataTextureArray outflows, IDataTextureArray contaminations, IDataTextureArray linkBarriers, IDataTextureArray flowLimits)
	{
		DispatchProcessColumns(depths.OldArray, columns.OldArray, linkBarriers.OldArray, outflows.OldArray, flowLimits.OldArray, _oldWaterData, _oldEdgeLinks, _oldCornerLinks, _oldBaseCornerLinks, _oldSkirts);
		DispatchProcessColumns(depths.NewArray, columns.NewArray, linkBarriers.NewArray, outflows.NewArray, flowLimits.NewArray, _newWaterData, _newEdgeLinks, _newCornerLinks, _newBaseCornerLinks, _newSkirts);
		DispatchFindBestProperties(outflows.OldArray, contaminations.OldArray, _oldWaterData, _oldEdgeLinks, _oldCornerLinksBuffer, _oldOutflows, _oldContaminations);
		DispatchFindBestProperties(outflows.NewArray, contaminations.NewArray, _newWaterData, _newEdgeLinks, _newCornerLinksBuffer, _newOutflows, _newContaminations);
		DispatchBreakCornerLinks(linkBarriers.NewArray, _oldEdgeLinks, _oldCornerLinks, _oldCornerLinksBuffer, _oldBrokenCornerLinks);
		DispatchBreakCornerLinks(linkBarriers.OldArray, _newEdgeLinks, _newCornerLinks, _newCornerLinksBuffer, _newBrokenCornerLinks);
		DispatchBreakCornerLinksReversed(_oldCornerLinksBuffer, _oldBrokenCornerLinks, _oldCornerLinks);
		DispatchBreakCornerLinksReversed(_newCornerLinksBuffer, _newBrokenCornerLinks, _newCornerLinks);
		DispatchVertexHeights(_oldWaterData, _oldEdgeLinks, _oldCornerLinks, _oldWaterHeights);
		DispatchVertexHeights(_newWaterData, _newEdgeLinks, _newCornerLinks, _newWaterHeights);
		if (_waterQualitySetting.HighQualityWaterEnabled)
		{
			DispatchFindWaterfalls(_oldWaterData, _oldWaterHeights, _oldWaterfalls);
			DispatchFindWaterfalls(_newWaterData, _newWaterHeights, _newWaterfalls);
		}
	}

	private void DispatchProcessColumns(Texture inDepths, Texture inColumns, Texture inLinkBarriers, Texture inOutflows, Texture inFlowLimits, Texture outWaterData, Texture outEdgeLinks, Texture outCornerLinks, Texture outBaseCornerLinks, Texture outSkirtVisibility)
	{
		SetTexture(_processColumnsKernel, InDepthsId, inDepths);
		SetTexture(_processColumnsKernel, InColumnsId, inColumns);
		SetTexture(_processColumnsKernel, InLinkBarriersId, inLinkBarriers);
		SetTexture(_processColumnsKernel, InOutflowsId, inOutflows);
		SetTexture(_processColumnsKernel, InFlowLimitsId, inFlowLimits);
		SetTexture(_processColumnsKernel, OutWaterDataId, outWaterData);
		SetTexture(_processColumnsKernel, OutEdgeLinksId, outEdgeLinks);
		SetTexture(_processColumnsKernel, OutCornerLinksId, outCornerLinks);
		SetTexture(_processColumnsKernel, OutBaseCornerLinksId, outBaseCornerLinks);
		SetTexture(_processColumnsKernel, OutSkirtsId, outSkirtVisibility);
		Dispatch(_processColumnsKernel, isTile: true);
	}

	private void DispatchFindBestProperties(Texture inOutflows, Texture inContaminations, Texture outWaterDataBuffer, Texture outEdgeLinksBuffer, Texture outCornerLinksBuffer, Texture outOutflows, Texture outContaminations)
	{
		SetTexture(_findBestPropertiesKernel, InOutflowsId, inOutflows);
		SetTexture(_findBestPropertiesKernel, InContaminationsId, inContaminations);
		SetTexture(_findBestPropertiesKernel, OutWaterDataBufferId, outWaterDataBuffer);
		SetTexture(_findBestPropertiesKernel, OutEdgeLinksBufferId, outEdgeLinksBuffer);
		SetTexture(_findBestPropertiesKernel, OutCornerLinksBufferId, outCornerLinksBuffer);
		SetTexture(_findBestPropertiesKernel, OutOutflowsId, outOutflows);
		SetTexture(_findBestPropertiesKernel, OutContaminationsId, outContaminations);
		Dispatch(_findBestPropertiesKernel, isTile: true);
	}

	private void DispatchBreakCornerLinks(Texture inLinkBarriers, Texture outEdgeLinksBuffer, Texture outCornerLinksBuffer, Texture outCornerLinks, Texture brokenCornerLinks)
	{
		SetTexture(_breakCornerLinksKernel, InLinkBarriersId, inLinkBarriers);
		SetTexture(_breakCornerLinksKernel, OutEdgeLinksBufferId, outEdgeLinksBuffer);
		SetTexture(_breakCornerLinksKernel, OutCornerLinksBufferId, outCornerLinksBuffer);
		SetTexture(_breakCornerLinksKernel, OutCornerLinksId, outCornerLinks);
		SetTexture(_breakCornerLinksKernel, BrokenCornerLinksId, brokenCornerLinks);
		Dispatch(_breakCornerLinksKernel, isTile: true);
	}

	private void DispatchBreakCornerLinksReversed(Texture outCornerLinksBuffer, Texture brokenCornerLinksBuffer, Texture outCornerLinks)
	{
		SetTexture(_breakCornerLinksReversedKernel, OutCornerLinksBufferId, outCornerLinksBuffer);
		SetTexture(_breakCornerLinksReversedKernel, BrokenCornerLinksBufferId, brokenCornerLinksBuffer);
		SetTexture(_breakCornerLinksReversedKernel, OutCornerLinksId, outCornerLinks);
		Dispatch(_breakCornerLinksReversedKernel, isTile: true);
	}

	private void DispatchVertexHeights(Texture outWaterDataBuffer, Texture outEdgeLinksBuffer, Texture outCornerLinksBuffer, Texture outHeights)
	{
		SetTexture(_calculateHeightsKernel, OutWaterDataBufferId, outWaterDataBuffer);
		SetTexture(_calculateHeightsKernel, OutEdgeLinksBufferId, outEdgeLinksBuffer);
		SetTexture(_calculateHeightsKernel, OutCornerLinksBufferId, outCornerLinksBuffer);
		SetTexture(_calculateHeightsKernel, OutHeightsId, outHeights);
		Dispatch(_calculateHeightsKernel, isTile: false);
	}

	private void DispatchFindWaterfalls(Texture outWaterDataBuffer, Texture outHeights, Texture outWaterfalls)
	{
		SetTexture(_findWaterfallsKernel, OutWaterDataBufferId, outWaterDataBuffer);
		SetTexture(_findWaterfallsKernel, OutHeightsBufferId, outHeights);
		SetTexture(_findWaterfallsKernel, OutWaterfallsId, outWaterfalls);
		Dispatch(_findWaterfallsKernel, isTile: true);
	}

	private void SetTexture(int kernel, int key, Texture texture)
	{
		_shader.SetTexture(kernel, key, texture);
	}

	private void Dispatch(int kernel, bool isTile)
	{
		if (isTile)
		{
			_shader.Dispatch(kernel, _tileThreadGroupCount.x, _tileThreadGroupCount.y, 1);
		}
		else
		{
			_shader.Dispatch(kernel, _vertexThreadGroupCount.x, _vertexThreadGroupCount.y, 1);
		}
	}

	private static int GetId(string name)
	{
		return Shader.PropertyToID(name);
	}

	private Vector3Int GetTileMapTextureSize()
	{
		return new Vector3Int(_mapIndexService.TerrainSize.x, _mapIndexService.TerrainSize.y, _arraySize);
	}

	private Vector3Int GetVertexMapTextureSize()
	{
		return new Vector3Int(4 * _mapIndexService.TerrainSize.x, 4 * _mapIndexService.TerrainSize.y, _arraySize);
	}
}
