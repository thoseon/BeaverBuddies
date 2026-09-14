using System.Collections.Generic;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Timberborn.SelectionSystem;

public class HighlightRenderingService : ScriptableRenderPass, ILoadableSingleton, IUnloadableSingleton
{
	private class PassData
	{
		internal RendererListHandle RendererList;
	}

	private static readonly List<ShaderTagId> ShaderTags = new List<ShaderTagId>
	{
		new ShaderTagId("UniversalForwardOnly"),
		new ShaderTagId("UniversalForward"),
		new ShaderTagId("SRPDefaultUnlit")
	};

	private static readonly int LayerId = RenderingLayerMask.NameToRenderingLayer("Selection");

	private static readonly string PassName = "RenderHighlightedObjects";

	private static readonly string HighlightMaskName = "_HighlightMask";

	private static readonly int HighlightMaskId = Shader.PropertyToID(HighlightMaskName);

	private readonly List<MeshRenderer> _renderersCache = new List<MeshRenderer>();

	private FilteringSettings _filteringSettings;

	public void Load()
	{
		_filteringSettings = new FilteringSettings(RenderQueueRange.all, -1, (uint)(1 << LayerId));
		base.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
		RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
	}

	public void Unload()
	{
		RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
	}

	public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer contextContainer)
	{
		PassData passData;
		using IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<PassData>(PassName, out passData, "C:\\workspace\\workspace\\p\\Assets\\Scripts\\Timberborn\\SelectionSystem\\HighlightRenderingService.cs", 37);
		passData.RendererList = CreateRendererList(renderGraph, contextContainer);
		UniversalResourceData universalResourceData = contextContainer.Get<UniversalResourceData>();
		TextureHandle highlightMask = GetHighlightMask(renderGraph, contextContainer);
		rasterRenderGraphBuilder.UseRendererList(in passData.RendererList);
		rasterRenderGraphBuilder.SetRenderAttachment(highlightMask, 0);
		rasterRenderGraphBuilder.SetRenderAttachmentDepth(universalResourceData.activeDepthTexture, AccessFlags.Read);
		rasterRenderGraphBuilder.SetGlobalTextureAfterPass(in highlightMask, HighlightMaskId);
		rasterRenderGraphBuilder.AllowPassCulling(value: true);
		rasterRenderGraphBuilder.SetRenderFunc(delegate(PassData data, RasterGraphContext context)
		{
			context.cmd.DrawRendererList(data.RendererList);
		});
	}

	public void AddToHighlight(GameObject root)
	{
		UpdateSelectionLayer(root, layerState: true);
	}

	public void RemoveFromHighlight(GameObject root)
	{
		UpdateSelectionLayer(root, layerState: false);
	}

	private void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
	{
		camera.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(this);
	}

	private RendererListHandle CreateRendererList(RenderGraph renderGraph, ContextContainer contextContainer)
	{
		UniversalCameraData universalCameraData = contextContainer.Get<UniversalCameraData>();
		UniversalRenderingData universalRenderingData = contextContainer.Get<UniversalRenderingData>();
		UniversalLightData lightData = contextContainer.Get<UniversalLightData>();
		DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(ShaderTags, universalRenderingData, universalCameraData, lightData, universalCameraData.defaultOpaqueSortFlags);
		return renderGraph.CreateRendererList(new RendererListParams(universalRenderingData.cullResults, drawSettings, _filteringSettings));
	}

	private static TextureHandle GetHighlightMask(RenderGraph renderGraph, ContextContainer contextContainer)
	{
		RenderTextureDescriptor cameraTargetDescriptor = contextContainer.Get<UniversalCameraData>().cameraTargetDescriptor;
		cameraTargetDescriptor.depthBufferBits = 0;
		return UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, HighlightMaskName, clear: true);
	}

	private void UpdateSelectionLayer(GameObject root, bool layerState)
	{
		root.GetComponentsInChildren(includeInactive: true, _renderersCache);
		foreach (MeshRenderer item in _renderersCache)
		{
			if (layerState)
			{
				item.renderingLayerMask |= (uint)(1 << LayerId);
			}
			else
			{
				item.renderingLayerMask &= (uint)(~(1 << LayerId));
			}
		}
		_renderersCache.Clear();
	}
}
