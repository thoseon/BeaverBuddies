using System.Collections.Generic;
using Timberborn.AssetSystem;
using Timberborn.PlatformUtilities;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Timberborn.WaterSystemRendering;

internal class WaterBackfacesRenderer : ScriptableRenderPass, ILoadableSingleton, IUnloadableSingleton
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

	private static readonly string MaterialPath = "Environment/Water/Materials/PhysicalWater_Backfaces";

	private static readonly string RendererPassName = "RenderWaterBackfaces";

	private static readonly int WaterLayerId = LayerMask.NameToLayer("Water");

	private readonly IAssetLoader _assetLoader;

	private FilteringSettings _filteringSettings;

	private Material _backfacesMaterial;

	public WaterBackfacesRenderer(IAssetLoader assetLoader)
	{
		_assetLoader = assetLoader;
	}

	public void Load()
	{
		_filteringSettings = new FilteringSettings(RenderQueueRange.all, 1 << WaterLayerId);
		_backfacesMaterial = _assetLoader.Load<Material>(MaterialPath);
		base.renderPassEvent = (RenderPassEvent)301;
		RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
	}

	public void Unload()
	{
		RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
	}

	public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer contextContainer)
	{
		PassData passData;
		using IRasterRenderGraphBuilder rasterRenderGraphBuilder = renderGraph.AddRasterRenderPass<PassData>(RendererPassName, out passData, "C:\\workspace\\workspace\\p\\Assets\\Scripts\\Timberborn\\WaterSystemRendering\\WaterBackfacesRenderer.cs", 46);
		passData.RendererList = CreateRendererList(renderGraph, contextContainer);
		UniversalResourceData universalResourceData = contextContainer.Get<UniversalResourceData>();
		rasterRenderGraphBuilder.UseRendererList(in passData.RendererList);
		if (ApplicationPlatform.IsMacOS())
		{
			RenderTextureDescriptor cameraTargetDescriptor = contextContainer.Get<UniversalCameraData>().cameraTargetDescriptor;
			cameraTargetDescriptor.depthBufferBits = 0;
			TextureHandle tex = UniversalRenderer.CreateRenderGraphTexture(renderGraph, cameraTargetDescriptor, "DummyColorTarget", clear: true);
			rasterRenderGraphBuilder.SetRenderAttachment(tex, 0);
		}
		rasterRenderGraphBuilder.SetRenderAttachmentDepth(universalResourceData.activeDepthTexture);
		rasterRenderGraphBuilder.SetRenderFunc(delegate(PassData data, RasterGraphContext context)
		{
			context.cmd.DrawRendererList(data.RendererList);
		});
	}

	private RendererListHandle CreateRendererList(RenderGraph renderGraph, ContextContainer contextContainer)
	{
		UniversalCameraData universalCameraData = contextContainer.Get<UniversalCameraData>();
		UniversalRenderingData universalRenderingData = contextContainer.Get<UniversalRenderingData>();
		UniversalLightData lightData = contextContainer.Get<UniversalLightData>();
		DrawingSettings drawSettings = RenderingUtils.CreateDrawingSettings(ShaderTags, universalRenderingData, universalCameraData, lightData, universalCameraData.defaultOpaqueSortFlags);
		drawSettings.overrideMaterial = _backfacesMaterial;
		return renderGraph.CreateRendererList(new RendererListParams(universalRenderingData.cullResults, drawSettings, _filteringSettings));
	}

	private void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
	{
		camera.GetUniversalAdditionalCameraData().scriptableRenderer.EnqueuePass(this);
	}
}
